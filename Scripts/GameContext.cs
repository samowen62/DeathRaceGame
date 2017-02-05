using UnityEngine;
using System.Collections;

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
