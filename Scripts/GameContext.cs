using UnityEngine;
using System.Collections;

public class GameContext : MonoBehaviour {

    public StartingSequence staringSequence;

    public RacePlayer player;

    public BackgroundScript pauseMenu;

    //will only read button once every second
    private const float buttonPressTime = 0.25f;

    private float spaceBarLastPressed;

    //used for controlling player input
    private PlayerInputDTO playerInput;

    void Awake () {

        playerInput = new PlayerInputDTO();

        spaceBarLastPressed = 0f;

        /**
         * set player.behaviorBlocked/staringSequence.behaviorBlocked to true/false 
         * in an actual game and false/true when testing to skip sequence
         */
        player.behaviorBlocked = false;

        staringSequence.behaviorBlocked = true;

        pauseMenu.paused = false;
    }
	
	// TODO: Handle inputs through this class only
	void Update () {
        handleInputs();

        if (staringSequence.finished && player.behaviorBlocked && !pauseMenu.paused)//Only change if player blocked (only should call once)
        {
            player.behaviorBlocked = false;
        }
	}

    private void handleInputs()
    {
        //pause game
        if (!pauseMenu.paused && Input.GetKey(KeyCode.Q) && (Time.fixedTime - spaceBarLastPressed > buttonPressTime))
        {
            pauseGame();
        }
        //un pause game
        else if(pauseMenu.paused && Input.GetKey(KeyCode.Q) && (Time.fixedTime - spaceBarLastPressed > buttonPressTime))
        {
            unpauseGame();
        }

 
        playerInput.setFromUser();
        player.passPlayerInputs(playerInput);
    }

    private void pauseGame()
    {
        Debug.Log("paused game");
        spaceBarLastPressed = Time.fixedTime;
        pauseMenu.paused = true;
        player.behaviorBlocked = true;
        staringSequence.behaviorBlocked = true;

        blockPrefabBehavior();
    }

    private void unpauseGame()
    {
        Debug.Log("un-paused game");
        spaceBarLastPressed = Time.fixedTime;
        pauseMenu.paused = false;

        unblockPrefabBehavior();

        staringSequence.behaviorBlocked = false;
        if (staringSequence.finished)
        {
            player.behaviorBlocked = false;
        }
    }

    private void blockPrefabBehavior()
    {
        var bannerPrefabs = FindObjectsOfType<BannerScroll>();

        foreach(var banner in bannerPrefabs)
        {
            banner.behaviorBlocked = true;
        }
    }

    private void unblockPrefabBehavior()
    {
        var bannerPrefabs = FindObjectsOfType<BannerScroll>();

        foreach (var banner in bannerPrefabs)
        {
            banner.behaviorBlocked = false;
        }
    }
}
