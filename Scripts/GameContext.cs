using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

public class GameContext : MonoBehaviour {

    public StartingSequence staringSequence;
    public RacePlayer playerMain;
    public Track track;
    public GameData gameData;
    private Dictionary<RacePlayer, PlacementDTO> racerPlacement;

    public Dictionary<RacePlayer, PlacementDTO> getracerPlacement()
    {
        return racerPlacement;
    }

    //TODO:Resources.Load doesn't work so I'm doing this instead in the meantime
    public PlacementFinishUI initialPlacementFinishUI;
    public Canvas canvas;
    public ProceedUI proceedUI;

    public int laps;
    public bool skipStart = false;

    //Highest placement still attainable
    private int finishPlacement = 1;

    private RacePlayer[] allPlayers;
    private GameObject[] pauseUIComponents;
    private PausableBehaviour[] pausableComponents;

    //will only read button once every second
    private const float buttonPressTime = 0.25f;

    //how many track points at a time the player can skip
    private const int skipTrackPoints = 10;

    private float pauseLastPressed;
    private bool paused;
    public bool debugging = false;

    void Awake () {
        pauseLastPressed = 0f;

        allPlayers = FindObjectsOfType<RacePlayer>();
        gameData = FindObjectOfType<GameData>();

        if(gameData == null)
        {
            Debug.LogWarning("No GameData object found!!");
        } else
        {
            proceedUI.gameData = gameData;
            foreach (var player in allPlayers)
            {
                if (!gameData.validatePlayerName(player.name))
                {
                    Debug.LogWarning(player.name + " not legal player name");
                }
            }
        }

        pauseUIComponents = GameObject.FindGameObjectsWithTag("PauseUI");
        pausableComponents = FindObjectsOfType<PausableBehaviour>();

        racerPlacement = new Dictionary<RacePlayer, PlacementDTO>();

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
            racerPlacement.Add(p, new PlacementDTO(laps));
        }

        staringSequence.finished = skipStart;
        staringSequence.seq_finished = skipStart;
        paused = false;

    }
	
	void Update () {
        handleInputs();

        findPlacement();

        //Start the race!
        if (staringSequence.finished && playerMain.behaviorBlocked && !paused)//Only change if player blocked (only should call once)
        {
            foreach (RacePlayer p in allPlayers)
            {
                p.behaviorBlocked = false;
                racerPlacement[p].lapStart[0] = staringSequence.time;
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

    /**
     * Determines order of player placement. (i.e. first through last place)
     */
    private void findPlacement()
    {
        if (!track.loaded) { 
            return;
        }

        int i = 1;

        var placement = racerPlacement
            .OrderByDescending(e => e.Value.lap)
            .ThenBy(e => e.Key.trackPointNumInSeq);

        foreach(var racer in placement)
        {
            racer.Key.placement = i;
            i++;
        }

        foreach (var player in allPlayers)
        {
            int playerTrackPoint = racerPlacement[player].latestTrackPoint;

            if(playerTrackPoint == -1) {
                if (player.trackPointNumInSeq == track.totalTrackPoints)
                {
                    playerTrackPoint = racerPlacement[player].latestTrackPoint = player.trackPointNumInSeq;
                }
                else //TODO:is this necessary?
                {
                    break;
                }
            }

            if (playerTrackPoint > player.trackPointNumInSeq)
            {
                if(player.trackPointNumInSeq > playerTrackPoint - skipTrackPoints)
                {
                    racerPlacement[player].latestTrackPoint = player.trackPointNumInSeq;
                    //Debug.Log(player.name + " || " + player.trackPointNumInSeq);
                }
            }
            else if (playerTrackPoint < skipTrackPoints)
            {
                //player passes the finish line
                if (player.passedFinish && !player.finished)
                {
                    finishLap(playerTrackPoint, player);
                }
            }
        }
    }

    public void pauseGame()
    {
        Debug.Log("paused game");
        pauseLastPressed = Time.fixedTime;
        paused = true;

        foreach (var component in pausableComponents)
        {
            component.behaviorBlocked = true;
        }

        foreach (var component in pauseUIComponents)
        {
            component.SetActive(true);
        }

    }

    public void unpauseGame()
    {
        Debug.Log("un-paused game");
        pauseLastPressed = Time.fixedTime;
        paused = false;

        foreach (var component in pauseUIComponents)
        {
            component.SetActive(false);
        }

        foreach (var component in pausableComponents)
        {
            component.behaviorBlocked = false;
        }

        if (!staringSequence.finished)
        {
            foreach (RacePlayer p in allPlayers)
            {
                p.behaviorBlocked = true;
            }
        }
    }

    private void finishLap(int playerTrackPoint, RacePlayer player)
    {
        player.passedFinish = false;
        racerPlacement[player].lap++;
        int lap = racerPlacement[player].lap;

        if (lap <= laps)
        {
            float time = staringSequence.time;

            racerPlacement[player].lapStart[lap - 1] = time;
            racerPlacement[player].lapTimes[lap - 2] = time - racerPlacement[player].lapStart[lap - 2];

            Debug.Log(player.name + " entered lap " + racerPlacement[player].lap + " total time: " + racerPlacement[player].lapTimes[lap - 2]);
            playerTrackPoint = track.totalTrackPoints;
        }
        //player finishes the race!!
        else if (lap - 1 == laps)
        {
            racerPlacement[player].lapTimes[laps - 1] = staringSequence.time - racerPlacement[player].lapStart[laps - 1];

            Debug.Log(player.name + " Finished!");
            player.finishRace();

            //null check if debugging on gameData
            if (!debugging)
            {
                gameData.addPlayerFinish(player.name, finishPlacement, racerPlacement[player].lapTimes);
                finishPlacement++;

                //if this is the main player or all but 1 have finished. Make other racers finish
                var unfinishedplayersOrdered = allPlayers.Where(e => !e.finished).OrderBy(e => e.placement);
                if (player.Equals(playerMain) || unfinishedplayersOrdered.Count() == 1)
                {
                    player.finishMainPlayer();
                    PlacementFinishUI[] playerUIFinish = new PlacementFinishUI[allPlayers.Length];

                    foreach (var p in unfinishedplayersOrdered)
                    {
                        fillEmptyLaps(p);
                        p.finishRace();
                        gameData.addPlayerFinish(p.name, finishPlacement, racerPlacement[p].lapTimes);
                        finishPlacement++;
                    }

                    RacePlayer[] allPlayersOrdered = allPlayers.OrderBy(e => gameData.getPlacement(e.name)).ToArray();
                    for (int i = 0; i < allPlayers.Length; i++)
                    {
                        string playerName = allPlayersOrdered[i].name;
                        PlacementFinishUI newFinishUI = createPlacementFinishUI(playerName,
                            gameData.getTotalTime(playerName), gameData.getPlacement(playerName),
                            (i + 1) * -50 );
                        //TODO:add these in pauseable array menu
                        pauseUIComponents.Concat(new GameObject[] { newFinishUI.gameObject });
                        pausableComponents.Concat(new PausableBehaviour[] { newFinishUI });
                        newFinishUI.startAnimation();
                    }

                    proceedUI.AppearAfterSeconds(initialPlacementFinishUI.waitingTime);

                    gameData.printContents();
                }
            }
        }
    }

    /**
     * Fills the lap times on an unfinished player. We will use the latest lap
     * time to populate the remaining laps
     */
    private void fillEmptyLaps(RacePlayer p)
    {
        float lastLapTime = 0f;
        float[] lapTimes = racerPlacement[p].lapTimes;
        for(int i = lapTimes.Length - 1; i >= 0; i--)
        {
            if(lapTimes[i] > 0)
            {
                lastLapTime = lapTimes[i];
                break;
            }
        }

        //TODO may want to factor in distance from finish line instead of just adding .15 [i.e. trackpoint num]
        float lapTime = staringSequence.time - lastLapTime + 0.15f;
        for (int i = lapTimes.Length - 1; i >= 0; i--)
        {
            if (lapTimes[i] > 0)
            {
                break;
            }else
            {
                lapTimes[i] = lapTime;
            }
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
