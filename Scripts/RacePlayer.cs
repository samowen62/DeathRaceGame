using UnityEngine;
using System.Collections;

/**
 * NOTES:
 * 
 *  1) To set the initial position and rotation of this object in unity let this player start for
 *     just a second or two and use the resulting orientation so that we take rigidBody motion into account
 */
public class RacePlayer : MonoBehaviour {

    //below are various parameters used to fine-tune game mechanics
    public float gravity = 10f;
    public float rayCastDistance = 30f;
    public float returningToTrackSpeed = 0.001f;
    public float timeAllowedNotOnTrack = 2.5f;
    public float timeSpentReturning = 3f;

    /*Ship handling parameters*/
    public float fwd_accel = 10f;
    public float fwd_max_speed = 20f;
    public float brake_speed = 20f;
    public float turn_speed = 5f;
    public float hard_turn_multiplier = 2.2f;
    public float air_turn_speed = 15f;

    /*Auto adjust to track surface parameters*/
    public float hover_height = 2f;     //Distance to keep from the ground
    public float height_smooth = 10f;   //How fast the ship will readjust to "hover_height"
    public float pitch_smooth = 5f;     //How fast the ship will adjust its rotation to match track normal
    public float height_correction = 3f;

    /*We will use all this stuff later*/
    private Vector3 disired_position;
    private Vector3 prev_up;
    private float yaw;
    private float smooth_y;
    private float current_speed;
  
    private Vector3 previousGravity;
    private RaycastHit downHit;

    /* Last checkpoint of the player */
    private CheckPoint lastCheckPoint;

    /* to only ray cast on ground layer objects*/
    private int bitMask = 1 << 8;

    /* This is for pausing the game */
    private bool _behaviorBlocked;
    public bool behaviorBlocked {
        get {
            return _behaviorBlocked;
        }
        set {
            if (value) {
                timePaused = Time.fixedTime;
            }
            else
            {
                //this is to ensure that pausing the game does not mess with timing
                //TODO: freeze the trail
                if (lastTimeOnGround != -1)
                {
                    lastTimeOnGround += (Time.fixedTime - timePaused);
                }             
                timeStartReturning += (Time.fixedTime - timePaused);
            }
            
            _behaviorBlocked = value;
        }
    }


    //this values will be effected by pausing the game
    private float timePaused;
    private float lastTimeOnGround;
    private float timeStartReturning;

    //TODO:stop using clumsy booleans and use an enum for the state of the player
    private bool returningToTrack = false;
    private bool falling = false;
    private Quaternion returningToTrackRotationBegin;
    private Vector3 returningToTrackPositionBegin;
    private Quaternion returningToTrackRotationEnd;
    private Vector3 returningToTrackPositionEnd;

    void Awake()
    {
        //TODO: sanity check to assert that the public parameters are within reasonable range (positive or negative)
        Track[] track = FindObjectsOfType(typeof(Track)) as Track[];
        if(track.Length != 1)
        {
            Debug.LogError("Error: only 1 component of type 'Track' allowed in the scene");
        }

        lastTimeOnGround = -1f;
        lastCheckPoint = track[0].startingCheckPoint;

        //TODO: Fix this to avoid jumping the rotation on start
        if (Physics.Raycast(transform.position, -transform.up, out downHit, rayCastDistance, bitMask))
        {
            transform.rotation = Quaternion.FromToRotation(transform.up, downHit.normal) * transform.rotation;
            yaw = transform.rotation.eulerAngles.y;
            previousGravity = -downHit.normal;
        }
    }

    void FixedUpdate()
    {
        if (_behaviorBlocked)
        {
            return;
        }

    
        if (returningToTrack)
        {
            float deltaTime = (Time.fixedTime - timeStartReturning) * returningToTrackSpeed;
            transform.position = Vector3.Lerp(returningToTrackPositionBegin, returningToTrackPositionEnd, deltaTime);
            transform.rotation = Quaternion.Lerp(returningToTrackRotationBegin, returningToTrackRotationEnd, deltaTime);
            returningToTrack = (Time.fixedTime - timeStartReturning) < timeSpentReturning;
            return;
        }

        
        prev_up = transform.up;

        /* Adjust the position and rotation of the ship to the track */
        if (Physics.Raycast(transform.position, -prev_up, out downHit, rayCastDistance, bitMask))
        {

            if (Input.GetKey(KeyCode.W))
            {
                current_speed += (current_speed >= fwd_max_speed) ? 0f : fwd_accel * Time.deltaTime;
            }
            else if (current_speed > 0)
            {
                current_speed -= brake_speed * Time.deltaTime;
            }
            else
            {
                current_speed = 0f;
            }

            turnShip(false);
            previousGravity = -downHit.normal;

            Vector3 desired_up = Vector3.Lerp(prev_up, downHit.normal, Time.deltaTime * pitch_smooth);
            Quaternion tilt = Quaternion.FromToRotation(transform.up, desired_up);
            transform.rotation = tilt * transform.rotation;

            /*Smoothly adjust our height*/
            smooth_y = Mathf.Lerp(smooth_y, hover_height - downHit.distance, Time.deltaTime * height_smooth);

            //sanity check on smooth_y
            smooth_y = Mathf.Max(downHit.distance / -3, smooth_y);

            transform.localPosition += prev_up * smooth_y;
            disired_position = transform.position + transform.forward * (current_speed * Time.deltaTime);

            if (Physics.Raycast(disired_position + height_correction * transform.up, -transform.up, out downHit, rayCastDistance, bitMask))
            {
                //this is so we do not fall through the track
                if (downHit.distance < height_correction + 0.1)
                {
                    Debug.Log("passed through" + downHit.distance);
                }
                else
                {
                    transform.position = disired_position;
                }
                
            }
        }
        else
        {
            /* only set this on the first frame that there is a miss */
            if (lastTimeOnGround == -1f)
            {
                Debug.Log("Player left contact with track");
                falling = true;
                lastTimeOnGround = Time.fixedTime;
            }
            /* called once to return player to the track*/
            else if ((Time.fixedTime - lastTimeOnGround) > timeAllowedNotOnTrack)
            {
                Debug.Log("Player returning to track");

                //set to -1 as a flag
                lastTimeOnGround = -1f;
                timeStartReturning = Time.fixedTime;
                returningToTrack = true;
                falling = false;
                current_speed = 0f;
                yaw = lastCheckPoint.yaw;

                returningToTrackRotationBegin = transform.rotation;
                returningToTrackPositionBegin = transform.position;
                returningToTrackRotationEnd = Quaternion.LookRotation(lastCheckPoint.trackForward, lastCheckPoint.trackNormal);
                returningToTrackPositionEnd = lastCheckPoint.trackPoint;
            }
            else
            {
                turnShip(true);
                transform.position += (gravity * Time.deltaTime * Time.deltaTime) * previousGravity + (transform.forward * (current_speed * Time.deltaTime));
            }
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        switch (coll.gameObject.tag)
        {
            //set the last checkpoint the player got to
            case "CheckPoint":
                lastCheckPoint = coll.gameObject.GetComponent<CheckPoint>();
                Debug.Log("Reached checkpoint " + lastCheckPoint.name);
                break;
            case "CollissionWall":
                Debug.Log("Hit wall");
                Debug.DrawRay(coll.ClosestPointOnBounds(transform.position), 20f * transform.forward, Color.green);
                break;
            default:
                Debug.LogWarning("No behavior for OnTriggerEnter with tag: " + coll.gameObject.tag);
                break;
        }
    }


    private void turnShip(bool inAir)
    {
        float turn_angle;
        if (inAir)
        {
            turn_angle = air_turn_speed * Time.deltaTime * Input.GetAxis("Horizontal");
        }
        else
        {
            turn_angle = turn_speed * Time.deltaTime * Input.GetAxis("Horizontal");
            if (Input.GetKey(KeyCode.Space))
            {
                turn_angle *= hard_turn_multiplier;
            }
        }      

        yaw += turn_angle;
        transform.rotation = Quaternion.Euler(0, yaw, 0);
    }

}
