using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class GameContext : MonoBehaviour {

    public StartingSequence staringSequence;

    public RacePlayer player;

    public Track track;

    private RacePlayer[] allPlayers;

    public Dictionary<RacePlayer, PlacementDTO> racerPlacement;

    public int laps;

    private GameObject[] pauseUIComponents;
    private PausableBehaviour[] pausableComponents;

    //will only read button once every second
    private const float buttonPressTime = 0.25f;

    //how many track points at a time the player can skip
    private const int skipTrackPoints = 10;

    private float pauseLastPressed;
    private bool paused;

    void Awake () {

        pauseLastPressed = 0f;

        allPlayers = FindObjectsOfType<RacePlayer>();
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


        /**
         * set allPlayers.behaviorBlocked/staringSequence.seq_finished to true/false 
         * in an actual game and false/true when testing to skip sequence
         */
        foreach (RacePlayer p in allPlayers)
        {
            p.behaviorBlocked = false;
            racerPlacement.Add(p, new PlacementDTO(laps));
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
                racerPlacement[p].lapStart[0] = staringSequence.time;
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
        //TODO:redo this. Save players that have finished
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

        foreach(var player in allPlayers)
        {
            int playerTrackPoint = racerPlacement[player].latestTrackPoint;

            if(playerTrackPoint == -1) {
                if (player.trackPointNumInSeq == track.totalTrackPoints)
                {
                    playerTrackPoint = racerPlacement[player].latestTrackPoint = player.trackPointNumInSeq;
                }
                else
                {
                    break;
                }
            }

            //if (player.name == "PlayerRacer")  Debug.Log(playerTrackPoint + " " + player.trackPointNumInSeq);
            if (playerTrackPoint > player.trackPointNumInSeq)
            {
                if(player.trackPointNumInSeq > playerTrackPoint - skipTrackPoints)
                {
                    racerPlacement[player].latestTrackPoint--;
                }
            }
            else if (playerTrackPoint < skipTrackPoints)
            {
                //player passes the finish line
                if (playerTrackPoint == 1 && player.passedFinish && !player.finished)
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
                    }
                }
                else
                {
                    //keep false until the finish line is passed in the few track points leading up to the finish
                    player.passedFinish = false;
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

}
