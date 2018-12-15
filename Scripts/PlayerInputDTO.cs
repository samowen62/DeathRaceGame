﻿using UnityEngine;

public class PlayerInputDTO  {

    public float horizonalAxis { private set; get; }
    public float verticalAxis { private set; get; }
    public bool sharpTurnButton { private set; get; }
    public bool pauseButton { private set; get; }
    public bool boostButton { private set; get; }
    public bool forwardButton { private set; get; }
    public bool attackButton { private set; get; }

    public PlayerInputDTO()
    {
        setToZero();
    }

    /*
     * Sets the inputs based off user input
     */
    public void setFromUser()
    {
        horizonalAxis = Input.GetAxis("Horizontal");
        verticalAxis = Input.GetAxis("Vertical");
        sharpTurnButton = Input.GetButton("Hard Drift");
        pauseButton = Input.GetButton("Pause Game");
        boostButton = Input.GetButton("Boost");
        forwardButton = verticalAxis > 0;
        attackButton = Input.GetButton("Side Attack");
    }

    public void setToZero()
    {
        horizonalAxis = 0f;
        verticalAxis = 0f;
        sharpTurnButton = false;
        pauseButton = false;
        boostButton = false;
        forwardButton = false;
        attackButton = false;
    }

}
