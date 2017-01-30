using UnityEngine;
using System.Collections;

/**
 * NOTES:
 * 
 *  1) To set the initial position and rotation of this object in unity let this player start for
 *     just a second or two and use the resulting orientation so that we take rigidBody motion into account
 */
public class RacePlayer : MonoBehaviour {

    //AI
    public bool isAI = true;

    //below are various parameters used to fine-tune game mechanics
    public float gravity = 10f;
    public float rayCastDistance = 30f;
    public float returningToTrackSpeed = 0.001f;
    public float timeAllowedNotOnTrack = 2.5f;
    public float timeSpentReturning = 3f;
    private float height_above_cast = 5f;

    /*Ship handling parameters, must be multiples of 5!! */
    public float fwd_accel = 10f;
    public float fwd_max_speed = 20f;
    public float fwd_boost_speed = 170f;
    public float fwd_boost_decel = 5f;

    public float brake_speed = 20f;
    public float turn_speed = 5f;
    public float hard_turn_multiplier = 2.2f;
    public float air_turn_speed = 15f;

    /*parameters for bouncing against the wall */
    public float wall_bounce_deccel = 10f;//must be > 1!!
    public float wall_bounce_threshold = 3f;
    public float wall_bounce_speed_to_bounce_ratio = 0.08f;
    public float wall_bounce_curr_speed_deccel = 10f;//must be > 1!!

    /*parameters for ship animation */
    public float ship_mesh_tilt = 7f;
    public float ship_mesh_tilt_hard_turn = 3f;

    /*Auto adjust to track surface parameters*/
    public float hover_height = AppConfig.hoverHeight - 0.5f;       //Distance to keep from the ground
    public float height_smooth = 10f;                               //How fast the ship will readjust to "hover_height"
    public float pitch_smooth = 5f;                                 //How fast the ship will adjust its rotation to match track normal
    public float height_correction = 3f;

    /*We will use all this stuff later*/
    private Vector3 disired_position;
    private Vector3 prev_up;
    private float yaw;
    private float smooth_y;
    private float current_speed;
  
    private Vector3 previousGravity;
    private RaycastHit downHit;
    private RaycastHit wallHit;

    private float wall_bounce_velocity = 0f;

    /* Last checkpoint of the player */
    private CheckPoint lastCheckPoint;
    private TrackPoint current_TrackPoint;
    private float prev_h = 0f;
    private float max_delta_h = 0.2f;

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
                //TODO: freeze the trail behind the engine
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

    private PlayerInputDTO player_inputs { set; get; }

    //renderer of ship
    private MeshRenderer shipRenderer;
    private Quaternion base_ship_rotation;

    void Awake()
    {
        player_inputs = new PlayerInputDTO();
        //(FindObjectsOfType(typeof(BoostPanel)) as BoostPanel[])[0].boostAnimation();

        //TODO: sanity check to assert that the public parameters are within reasonable range (positive or negative)

        //TODO: abstract finding 1 object of type and 1 object of name in general util class with assertion
        Track[] track = FindObjectsOfType(typeof(Track)) as Track[];
        if(track.Length != 1)
        {
            Debug.LogError("Error: only 1 component of type 'Track' allowed in the scene");
        }

        shipRenderer = transform.FindChild("Ship").gameObject.GetComponent<MeshRenderer>();
        if (shipRenderer == null)
        {
            Debug.LogError("Please name the ship prefab 'Ship' in this instance of RacePlayer.cs");
        }
        base_ship_rotation = shipRenderer.transform.localRotation;

        lastTimeOnGround = -1f;
        lastCheckPoint = track[0].startingCheckPoint;

        //TODO: Fix this to avoid jumping the rotation on start. Maybe just give current speed on start?
        if (Physics.Raycast(transform.position, -transform.up, out downHit, rayCastDistance, AppConfig.groundMask))
        {
            
            transform.position = downHit.point + hover_height * transform.up;
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

        
        if(wall_bounce_velocity != 0)
        {
            transform.position += wall_bounce_velocity * transform.right;
            wall_bounce_velocity /= wall_bounce_deccel;

            if(Mathf.Abs(wall_bounce_velocity) < wall_bounce_threshold)
            {
                wall_bounce_velocity = 0;
            }
        }

        bool accelerating = isAI ? true : player_inputs.w_key;
        prev_up = transform.up;

        /* Adjust the position and rotation of the ship to the track */
        if (Physics.Raycast(transform.position + height_above_cast * prev_up, -prev_up, out downHit, rayCastDistance, AppConfig.groundMask))
        {

            if (accelerating)
            {
                current_speed += (current_speed >= fwd_max_speed) ? ((current_speed == fwd_max_speed) ? 0 : -fwd_boost_decel) : fwd_accel * Time.deltaTime;
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

            //Smoothly adjust our height
            float distance = downHit.distance - height_above_cast;
            smooth_y = Mathf.Lerp(smooth_y, hover_height - distance, Time.deltaTime * height_smooth);

            //sanity check on smooth_y
            smooth_y = Mathf.Max(distance / -3, smooth_y);

            transform.localPosition += prev_up * smooth_y;

            transform.position += transform.forward * (current_speed * Time.deltaTime);

        }
        else
        {
            /* only set this on the first frame that there is a miss */
            if (lastTimeOnGround == -1f)
            {
                Debug.Log("Player left contact with track");
                falling = true;
                lastTimeOnGround = Time.fixedTime;
                wall_bounce_velocity = 0;
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

            //create a velocity vector from bouncing off the wall
            case "CollissionWall":

                if (Physics.Raycast(transform.position, -transform.right, out wallHit, rayCastDistance, AppConfig.wallMask))
                {
                    wall_bounce_velocity = -wall_bounce_speed_to_bounce_ratio * current_speed * Vector3.Dot(wallHit.normal, transform.forward);
                    current_speed /= wall_bounce_curr_speed_deccel;
                }
                else if (Physics.Raycast(transform.position, transform.right, out wallHit, rayCastDistance, AppConfig.wallMask))
                {
                    wall_bounce_velocity = current_speed * wall_bounce_speed_to_bounce_ratio * Vector3.Dot(wallHit.normal, transform.forward);
                    current_speed /= wall_bounce_curr_speed_deccel;
                } else
                {
                    Debug.LogWarning("Detected hit with wall but cannot find wall on left or right!");
                }
                break;

            //when we hit a boost panel
            case "BoostPanel":
                Debug.Log("Boost Power");
                current_speed = fwd_boost_speed;

                //trigger the animation
                coll.gameObject.GetComponent<BoostPanel>().boostAnimation();
                break;

            //when we hit a trackpoint trigger
            case "TrackPoint":
                current_TrackPoint = coll.gameObject.GetComponent<TrackPoint>();
                break;

            //Log warning for unhandled tag
            default:
                Debug.LogWarning("No behavior for OnTriggerEnter with tag: " + coll.gameObject.tag);
                break;
        }
    }

    /*
     * Needed to pass inputs to the player
     */
    public void passPlayerInputs(PlayerInputDTO _player_inputs)
    {
        player_inputs = _player_inputs;
    }

    private void turnShip(bool inAir)
    {

        //TODO: refactor for different dev modes
        float horizontal_input;
        bool spaceBar = false;
        if (!isAI)
        {
            horizontal_input = player_inputs.horizonalAxis;
            spaceBar = player_inputs.spaceBar;
            prev_h = horizontal_input;
        }
        else
        {
            
            if (current_TrackPoint == null)
            {
                horizontal_input = player_inputs.horizonalAxis;
            }
            else
            {
                setInputsFromAI(out horizontal_input, out spaceBar);
            }
        }
   

        float turn_angle = 0f;
        if (inAir)
        {
            turn_angle = air_turn_speed * Time.deltaTime * horizontal_input;
        }
        else
        {
            turn_angle = turn_speed * Time.deltaTime * horizontal_input;
            if (spaceBar)
            {
                turn_angle *= hard_turn_multiplier;
                shipRenderer.transform.localRotation = Quaternion.AngleAxis(ship_mesh_tilt_hard_turn * turn_angle, shipRenderer.transform.forward) * base_ship_rotation;
            }
            else
            {
                shipRenderer.transform.localRotation = Quaternion.AngleAxis(ship_mesh_tilt * turn_angle, shipRenderer.transform.forward) * base_ship_rotation;
            }
        }      

        yaw += turn_angle;
        transform.rotation = Quaternion.Euler(0, yaw, 0);
    }

    private void setInputsFromAI(out float horizontal_input, out bool spaceBar)
    {
        //for h - is turning left and + is right!
        spaceBar = false;
        int nearEdge = AIUtil.checkIfNearEdge(transform.position, transform.up, current_TrackPoint);

        //Need to turn the ship away from the edge
        if(nearEdge != 0)
        {
            spaceBar = true;
            if(nearEdge > 0)
            {
                horizontal_input = prev_h + max_delta_h;
            }
            else
            {
                horizontal_input = prev_h - max_delta_h;
            }
            horizontal_input = horizontal_input > 1 ? 1 : horizontal_input;
            horizontal_input = horizontal_input < -1 ? -1 : horizontal_input;

            prev_h = horizontal_input;
            return;
        }

        //TODO: make more presumptive (look N = 3 spots ahead? then see how they compare to 1 and hard_turn_multiplier)
        //adjust tangent for N spots ahead or calculate h for each. (the former has less calculations and doesn't lose much info)

        //first check tangents of next and next.next? trackpoint. If their dot is small enough, don't do anything

        //weight the farther away ones more
        float h3 = AIUtil.getHorizontal(transform.position, transform.forward, current_speed, current_TrackPoint.next.next.next);
        float h2 = AIUtil.getHorizontal(transform.position, transform.forward, current_speed, current_TrackPoint.next.next);
        float h1 = AIUtil.getHorizontal(transform.position, transform.forward, current_speed, current_TrackPoint.next);

        //TODO: play with these weight factors until we get it right!
        horizontal_input = 0.25f * h1 + 0.25f * h2 + 0.5f * h3;

        //Debug.Log(h + "  " + h1 + " " + h2 + " " + h3);
        if (horizontal_input > hard_turn_multiplier)
        {
            spaceBar = true;
        }
        horizontal_input = Mathf.Clamp01(horizontal_input);

        //TODO may have to factor in isTrackReversed here as well (Track.cs property) for the > sign
        int sign = Vector3.Dot(Vector3.Cross(transform.forward, transform.up), current_TrackPoint.tangent) > 0 ? -1 : 1;

        horizontal_input *= sign;

        //Debug.Log("sign:" + sign + " h:" + h + " prev_h:" + prev_h);

        if (horizontal_input - prev_h >= max_delta_h)
            horizontal_input = prev_h + max_delta_h;
        else if (prev_h - horizontal_input >= max_delta_h)
            horizontal_input = prev_h - max_delta_h;

        //TODO inline function for clamping between +/- 1
        horizontal_input = horizontal_input > 1 ? 1 : horizontal_input;
        horizontal_input = horizontal_input < -1 ? -1 : horizontal_input;

        prev_h = horizontal_input;

    }
}
