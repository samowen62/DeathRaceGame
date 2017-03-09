using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GameContext : MonoBehaviour {

    public StartingSequence staringSequence;

    public RacePlayer player;

    public Track track;

    public RacePlayer[] allPlayers;
    private GameObject[] pauseUIComponents;
    private PausableBehaviour[] pausableComponents;

    //will only read button once every second
    private const float buttonPressTime = 0.25f;

    private float pauseLastPressed;
    private bool paused;

    void Awake () {

        pauseLastPressed = 0f;

        allPlayers = FindObjectsOfType<RacePlayer>();
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


        /**
         * set allPlayers.behaviorBlocked/staringSequence.seq_finished to true/false 
         * in an actual game and false/true when testing to skip sequence
         */
        foreach (RacePlayer p in allPlayers)
        {
            p.behaviorBlocked = false;
        }

        staringSequence.finished = true;
        staringSequence.seq_finished = true;
        paused = false;
    }
	
	void Update () {
        handleInputs();

        findPlacement();

        if (staringSequence.finished && player.behaviorBlocked && !paused)//Only change if player blocked (only should call once)
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
        if (!paused && Input.GetKey(KeyCode.Q) && (Time.fixedTime - pauseLastPressed > buttonPressTime))
        {
            pauseGame();
        }
        //un pause game
        else if(paused && Input.GetKey(KeyCode.Q) && (Time.fixedTime - pauseLastPressed > buttonPressTime))
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

        //add laps to OrderBy
        int i = 1;
        allPlayers
            .GroupBy(e => e.trackPointNumInSeq)
            .OrderBy(e => e.First().trackPointNumInSeq)
            .ToList().ForEach(racers =>
        {
            if(racers.Count() > 1)
            {
                racers
                    .OrderBy(e => e.depthInTrackPoint())
                    .ToList()
                    .ForEach(racer =>
                        {
                            racer.placement = i;
                            i++;
                        });
            }else
            {
                racers.First().placement = i;
                i++;
            }
            
        });     
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

}
