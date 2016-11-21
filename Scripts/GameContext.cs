using UnityEngine;
using System.Collections;

public class GameContext : MonoBehaviour {

    public StartingSequence staringSequence;

    public RacePlayer player;

    public BackgroundScript pauseMenu;

    //will only read button once every second
    private const float buttonPressTime = 0.25f;

    private float spaceBarLastPressed;

    void Awake () {

        spaceBarLastPressed = 0f;

        player.behaviorBlocked = true;

        staringSequence.behaviorBlocked = false;

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
            Debug.Log("paused game");
            spaceBarLastPressed = Time.fixedTime;
            pauseMenu.paused = true;
            player.behaviorBlocked = true;
            staringSequence.behaviorBlocked = true;
        }
        //un pause game
        else if(pauseMenu.paused && Input.GetKey(KeyCode.Q) && (Time.fixedTime - spaceBarLastPressed > buttonPressTime))
        {
            Debug.Log("un-paused game");
            spaceBarLastPressed = Time.fixedTime;
            pauseMenu.paused = false;
            
            staringSequence.behaviorBlocked = false;
            if (staringSequence.finished)
            {
                player.behaviorBlocked = false;
            }
        }
    }
}
