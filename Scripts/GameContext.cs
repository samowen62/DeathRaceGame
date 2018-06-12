using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class GameContext : MonoBehaviour {

    public StartingSequence startingSequence;
    public RacePlayer playerMain;
    public Track track;
    public PlacementManager placementManager;
    public GameData gameData;

    //TODO:Resources.Load doesn't work so I'm doing this instead in the meantime
    public PlacementFinishUI initialPlacementFinishUI;
    public Canvas canvas;
    public ProceedUI proceedUI;
    public NewRecordUI newRecordUI;

    public bool skipStart = false;

    private RacePlayer[] allPlayers;
    private GameObject[] pauseUIComponents;
    private PausableBehaviour[] pausableComponents;

    //will only read button once every second
    private const float buttonPressTime = 0.25f;

    //how many track points at a time the player can skip
    private const int skipTrackPoints = 10;

    private float pauseLastPressed;
    private bool paused;
    private bool gameFinished = false;

    void Awake () {
        pauseLastPressed = 0f;

        allPlayers = FindObjectsOfType<RacePlayer>();
        gameData = FindObjectOfType<GameData>();

        placementManager.addPlayers(allPlayers.ToList());

        if(gameData == null)
        {
            Debug.LogWarning("No GameData object found!!");
        } else
        {
            proceedUI.gameData = gameData;          
        }

        foreach (var player in allPlayers)
        {
            if (!(gameData == null || gameData.validatePlayerName(player.name)))
            {
                Debug.LogWarning(player.name + " not legal player name");
            }

            /* find the main player and set all objects accordingly */
            if ((gameData != null && gameData.mainPlayer.Equals(player.name))
                || (gameData == null && player == playerMain))
            {
                player.isAI = false;
                playerMain = player;
                startingSequence.mainRacer = player;

                //TODO:find neater way of doing this!!!
                canvas.transform.Find("Placement").GetComponent<PlacementUI>().player = player;
                canvas.transform.Find("HealthBar").GetComponent<HealthUI>().player = player;
                canvas.transform.Find("LapTimes").GetComponent<LapsUI>().player = player;
                canvas.transform.Find("Speed").GetComponent<MPHUI>().player = player;
                canvas.transform.Find("CenterTextBG").GetComponent<CenterTextUI>().player = player;

                AppConfig.changeParent(player.gameObject, Camera.main.gameObject);
            }
            else
            {
                player.isAI = true;
            }
        }

        pauseUIComponents = GameObject.FindGameObjectsWithTag("PauseUI");
        pausableComponents = FindObjectsOfType<PausableBehaviour>();

        foreach (var component in pausableComponents)
        {
            component.behaviorBlocked = false;
        }

        foreach (var component in pauseUIComponents)
        {
            component.SetActive(false);
        }

        foreach (RacePlayer p in allPlayers)
        {
            p.behaviorBlocked = !skipStart;
        }

        startingSequence.finished = skipStart;
        startingSequence.seq_finished = skipStart;
        paused = false;

    }
	
	void Update () {
        handleInputs();

        //Start the race!
        if (startingSequence.finished && playerMain.behaviorBlocked && !paused)//Only change if player blocked (only should call once)
        {
            foreach (RacePlayer p in allPlayers)
            {
                p.behaviorBlocked = false;
            }
        }
	}

    private void handleInputs()
    {
        //pause game
        if (!paused && Input.GetKey(AppConfig.PAUSE_BUTTON) && (Time.fixedTime - pauseLastPressed > buttonPressTime))
        {
            pauseGame();
        }
        //un pause game
        else if(paused && Input.GetKey(AppConfig.PAUSE_BUTTON) && (Time.fixedTime - pauseLastPressed > buttonPressTime))
        {
            unpauseGame();
        }

    }


    public void pauseGame()
    {
        Debug.Log("paused game");
        pausePressed(true);
    }

    public void unpauseGame()
    {
        Debug.Log("un-paused game");
        pausePressed(false);

        if (!startingSequence.finished)
        {
            foreach (RacePlayer p in allPlayers)
            {
                p.behaviorBlocked = true;
            }
        }
    }

    private void pausePressed(bool pause)
    {
        pauseLastPressed = Time.fixedTime;
        paused = pause;

        foreach (var component in pauseUIComponents)
        {
            component.SetActive(pause);
        }

        foreach (var component in pausableComponents)
        {
            component.behaviorBlocked = pause;
        }
    }

    /**
     * called from the placement manager once a racer crosses the finish line. All logic 
     * related to finishes are handled here
     */
    public void finishPlayer(RacePlayer player)
    {
        if (player.finished) return;

        gameData.addPlayerFinish(player.name, 
            placementManager.getLapTimesForPlayer(player));

        if (player.isAI)
        {
            player.finishRace();
        }

        var unfinishedplayersOrdered = placementManager.getUnfinishedPlayersOrdered();

        //Finish race if main finishes or all else are
        if (!gameFinished && (player.Equals(playerMain) || unfinishedplayersOrdered.Count() == 1))
        {
            gameFinished = true;
            player.finishMainPlayer();
            PlacementFinishUI[] playerUIFinish = new PlacementFinishUI[allPlayers.Length];

            foreach (var unfinishedPlayer in unfinishedplayersOrdered)
            {                
                placementManager.forcePlayerFinish(unfinishedPlayer);
                gameData.addPlayerFinish(unfinishedPlayer.name, 
                    placementManager.getLapTimesForPlayer(player));
                if (!(player.Equals(playerMain))){
                    unfinishedPlayer.finishRace();
                }
            }

            // show "New Record!" if player set a record
            if (gameData.TrySaveRaceRecords(playerMain.name))
            {
                newRecordUI.Activate();
            }

            RacePlayer[] allPlayersOrdered = allPlayers.OrderBy(e => gameData.getPlacement(e.name)).ToArray();
            for (int i = 0; i < allPlayersOrdered.Length; i++)
            {
                string playerName = allPlayersOrdered[i].name;
                PlacementFinishUI newFinishUI = createPlacementFinishUI(
                    AppConfig.getRacerDisplayName(playerName),
                    gameData.getTotalTime(playerName),
                    gameData.getPlacement(playerName),
                    (i + 1) * -50);
                //TODO:add these in pauseable array menu
                pauseUIComponents.Concat(new GameObject[] { newFinishUI.gameObject });
                pausableComponents.Concat(new PausableBehaviour[] { newFinishUI });
                newFinishUI.startAnimation();
            }

            //scene
            proceedUI.AppearAfterSeconds(initialPlacementFinishUI.waitingTime);

            gameData.printContents();
        }
    }

    private PlacementFinishUI createPlacementFinishUI(string _playerTxt, string _lapTimeTxt,
        string _placementNumberTxt, float _downwardDistance)
    {
        //TODO:add this new prefab to the pausable prefab list
        PlacementFinishUI placFinishUI = Instantiate(
            initialPlacementFinishUI, 
            canvas.transform.position + new Vector3(40, 20, 0), 
            Quaternion.identity) as PlacementFinishUI;
        placFinishUI.transform.parent = canvas.transform;
        placFinishUI.setPrams(_playerTxt, _lapTimeTxt, _placementNumberTxt, _downwardDistance);
        return placFinishUI;
    }

}
