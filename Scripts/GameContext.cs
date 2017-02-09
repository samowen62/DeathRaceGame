using UnityEngine;
using System.Collections;
using System.Linq;

public class GameContext : MonoBehaviour {

    public StartingSequence staringSequence;

    public RacePlayer player;

    public RacePlayer[] allPlayers;
    private BannerScroll[] bannerPrefabs;

    public BackgroundScript pauseMenu;

    //will only read button once every second
    private const float buttonPressTime = 0.25f;

    private float pauseLastPressed;

    void Awake () {

        pauseLastPressed = 0f;

        allPlayers = FindObjectsOfType<RacePlayer>();
        bannerPrefabs = FindObjectsOfType<BannerScroll>();

        /**
         * set allPlayers.behaviorBlocked/staringSequence.behaviorBlocked to true/false 
         * in an actual game and false/true when testing to skip sequence
         */

        foreach (RacePlayer p in allPlayers)
        {
            p.behaviorBlocked = true;
        }

        staringSequence.behaviorBlocked = false;

        pauseMenu.paused = false;

    }
	
	void Update () {
        handleInputs();

        findPlacement();

        if (staringSequence.finished && player.behaviorBlocked && !pauseMenu.paused)//Only change if player blocked (only should call once)
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
        if (!pauseMenu.paused && Input.GetKey(KeyCode.Q) && (Time.fixedTime - pauseLastPressed > buttonPressTime))
        {
            pauseGame();
        }
        //un pause game
        else if(pauseMenu.paused && Input.GetKey(KeyCode.Q) && (Time.fixedTime - pauseLastPressed > buttonPressTime))
        {
            unpauseGame();
        }

    }

    /**
     * Determines order of player placement. (i.e. first through last place)
     */
    private void findPlacement()
    {
        //add laps to OrderBy
        int i = 1;
        allPlayers
            .GroupBy(e => e.player_TrackPoint.num_in_seq)
            .OrderBy(e => e.First().player_TrackPoint.num_in_seq)
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

    private void pauseGame()
    {
        Debug.Log("paused game");
        pauseLastPressed = Time.fixedTime;
        pauseMenu.paused = true;

        foreach(RacePlayer p in allPlayers)
        {
            p.behaviorBlocked = true;
        }

        staringSequence.behaviorBlocked = true;

        blockPrefabBehavior();
    }

    private void unpauseGame()
    {
        Debug.Log("un-paused game");
        pauseLastPressed = Time.fixedTime;
        pauseMenu.paused = false;

        unblockPrefabBehavior();

        staringSequence.behaviorBlocked = false;
        if (staringSequence.finished)
        {
            foreach (RacePlayer p in allPlayers)
            {
                p.behaviorBlocked = false;
            }
        }
    }

    private void blockPrefabBehavior()
    {
        foreach(var banner in bannerPrefabs)
        {
            banner.behaviorBlocked = true;
        }
    }

    private void unblockPrefabBehavior()
    {
        foreach (var banner in bannerPrefabs)
        {
            banner.behaviorBlocked = false;
        }
    }
}
