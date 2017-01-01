using UnityEngine;
using System.Collections;

public class PlayerInputDTO  {

    public float horizonalAxis { private set; get; }
    public bool spaceBar { private set; get; }
    public bool pauseButton { private set; get; }
    public bool w_key { private set; get; }

    public PlayerInputDTO()
    {
        horizonalAxis = 0f;
        spaceBar = false;
        pauseButton = false;
        w_key = false;
    }

    /*
     * Sets the inputs based off user input
     */
    public void setFromUser()
    {
        horizonalAxis = Input.GetAxis("Horizontal");
        spaceBar = Input.GetKey(KeyCode.Space);
        pauseButton = Input.GetKey(KeyCode.Q);
        w_key = Input.GetKey(KeyCode.W);
    }

    //TODO: ALOT!!!!!!!
    public void setFromAI()
    {

    }

}
