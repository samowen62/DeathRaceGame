using UnityEngine;

public class PlayerInputDTO  {

    public float horizonalAxis { private set; get; }
    public float verticalAxis { private set; get; }
    public bool spaceBar { private set; get; }
    public bool pauseButton { private set; get; }
    public bool w_key { private set; get; }
    public bool e_key { private set; get; }

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
        spaceBar = Input.GetKey(KeyCode.Space);
        pauseButton = Input.GetKey(KeyCode.Q);
        w_key = Input.GetKey(KeyCode.W);
        e_key = Input.GetKey(KeyCode.E);
    }

    public void setToZero()
    {
        horizonalAxis = 0f;
        verticalAxis = 0f;
        spaceBar = false;
        pauseButton = false;
        w_key = false;
        e_key = false;
    }

}
