using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;

public class RacePlayer : PausableBehaviour
{
    //TODO: to organize these private varibles:
    // 1) organize and indent based on section
    // 2) change some to const or private. Waaay too many publics
    // 3) change some to private set with public get where needed


    /* AI indicator */
    public bool isAI = true;
    public TrackPoint.PathChoice AIPathChoice = TrackPoint.PathChoice.PATH_A;


    /* Related to air and returning mechanics */
    public float gravity = 1700f;
    public float returningToTrackSpeed = 0.8f;
    public float timeAllowedNotOnTrack = 2f;
    public float timeSpentReturning = 1.5f;


    /*Ship handling parameters, must be multiples of 5!! */
    public float fwd_accel = 80f;
    public float fwd_max_speed = 130f;
    public float fwd_boost_speed = 170f;
    public float fwd_boost_decel = 2.5f;
    public float boost_time_window = 2.2f;
    public int boost_cost = 10;
    public AudioObject boostSound;

    public float brake_speed = 200f;
    public float turn_speed = 80f;
    public float hard_turn_multiplier = 2.2f;
    private float air_turn_speed = 10f;

    /* Related to for bouncing against the wall */
    public float wall_bounce_deccel = 10f;//must be > 1!!
    public float wall_bounce_threshold = 5f;
    public float wall_bounce_speed_to_bounce_ratio = 0.1f;
    public float wall_bounce_curr_speed_deccel = 2f;//must be > 1!!

    /* Related to for ship animation */
    public float ship_mesh_tilt = 5f;
    public float ship_mesh_tilt_hard_turn = 3f;

    /* Related to ship orientation and sticking to the track*/
    public float hover_height = AppConfig.hoverHeight - 0.5f;
    public float height_smooth = 7f;                               //How fast the ship will readjust to "hover_height"
    public float pitch_smooth = 5f;                                //How fast the ship will adjust its rotation to match track normal
    public float height_correction = 2.2f;
    private float rayCastDistance = 10f;
    private float freeFallRayCastDistance = 50f;//50 is good!
    private Vector3 disired_position;
    private Vector3 prev_up;
    private Quaternion global_orientation;
    private Quaternion tilt;
    private float smooth_y;
    private float height_above_cast = 5f;
    private float current_speed;
    private float downward_speed = 0f;

    //TODO: get more accurate reading of actual speed by measuring points
    public float speed
    {
        get
        {
            return current_speed;
        }
    }

    private Vector3 previousGravity;
    private RaycastHit downHit;
    private RaycastHit wallHit;

    private Vector3 wall_bounce_velocity = Vector3.zero;
    private PlayerStatus status = PlayerStatus.ONTRACK;

    /* Related to player gliding */
    private bool inFreefall;
    private float totalPitch;
    private float max_pitch = 40;
    private float min_pitch = -20;
    private float pitch_per_vert = 2f;
    private float air_speed_damping = 0.6f;
    private float pitch_decel = 10f;

    /* Related to player effects */
    private Explosion explosion;
    private SwipeTrail swipeTrail;
    private ElectricalEffect electricalEffect;

    private Dictionary<string, RacePlayer> playersToAttack;
    private Vector3 attack_velocity = Vector3.zero;
    private Vector3 attacked_velocity = Vector3.zero;
    public AudioObject bumpSound;
    private float attack_time_window = 0.25f;
    public float attack_deccel = 10;
    public float attack_threshold = 5;
    public float attack_magnitude = 5;
    public float attacked_magnitude = 2;
    public float attack_damage_multiplier = 0.3f;
    private float attack_bump_damage = 1f;
    private float attack_bump_magnitude = 0.1f;
    private float attack_damage_transfer_factor = -0.8f;
    private float totalRoll;
    private float roll_decel = 1.4f;
    private float attack_roll = 25f;
    private bool dead = false;
    public bool isDead {
        get
        {
            return dead;
        }
    }
    private float timeCameraShake = 1.7f;
    private float timeCameraShakeBump = 0.2f;
    private float timeAllowedDead = 4f;
    private float gyrationFactor = 0.2f;
    private bool cameraIsShaking = false;

    /* Related to player health */
    private Material shipMaterial;
    private Material redMaterial;
    private Color baseTint = new Color(0f, 0f, 0f, 1.0f);
    private Color tintColor = new Color(0f, 1f, 0.2f, 1.0f);
    public float starting_health = 100f;
    public float max_bonus_health = 150f;
    public float health_per_frame_healing = 0.2f;
    public float health_blink_speed = 20f;
    private bool over_healing_area = false;
    private float health_warning_thresh = 25f;
    private float player_health;
    public float health
    {
        get
        {
            return player_health;
        }
    }

    private PlacementManager placementManager;
    private GameEventsUI gameEventsUI;

    /* Last checkpoint of the player */
    private Vector3 lastCheckPointUp;
    private Vector3 lastCheckPointPosition;
    private TrackPoint lastCheckPoint;
    private TrackPoint current_TrackPoint;
 
    public bool startedLap1 { get; private set; }
    private bool finishedWithRace = false;
    public bool finished
    {
        get
        {
            return finishedWithRace;
        }
    }
    private bool isEffectiveAI {
        get
        {
            return finishedWithRace || isAI;
        }
    }
    private Vector3 finishedCameraPosition = new Vector3(-3.75f, 9.15f, 10.45f);
    private Quaternion finishedCameraRotation = Quaternion.Euler(39.53f, 172.45f, 6.1f);

    private float prev_h = 0f;
    private float max_delta_h = 0.2f;
    private float prev_v = 0f;
    private float max_delta_v = 0.001f;

    private float lastTimeOnGround = 0f;
    private float lastTimeAttacked = 0f;
    private float lastTimeBoostPower = 0f;
    private float timeStartReturning = 0f;
    private float timeStartDeath = 0f;

    private float boostPlayerToCamera_Z = 6f; //how much farther away the camera is during boost
    private float boostCameraSpeed = 0.7f;
    private float boostCameraDistance = 0f;

    //TODO: make this some kind of game constant
    private Vector3 _playerToCamera = new Vector3(0, 10, -20);
    public BezierSpline CameraPath { get; set; }
    public Quaternion cameraRotation { get { return Quaternion.Euler(6, 0, 0); }}
    public Vector3 playerToCamera
    {
        get
        {
            if (finishedWithRace){
                return finishedCameraPosition;
            }
            if(current_speed <= fwd_max_speed)
            {
                return _playerToCamera;
            }
            float target_z_distance = Mathf.Lerp(0, boostPlayerToCamera_Z, 
                (current_speed - fwd_max_speed)/ (fwd_boost_speed - fwd_max_speed));
            //control how fast the camera can zoom out
            if(target_z_distance - boostCameraDistance > boostCameraSpeed)
            {
                boostCameraDistance += boostCameraSpeed;
            } else if (boostCameraDistance - target_z_distance > boostCameraSpeed)
            {
                boostCameraDistance -= boostCameraSpeed;
            } else
            {
                boostCameraDistance = target_z_distance;
            }
            return _playerToCamera - new Vector3(0, 0, boostCameraDistance);
        }
    }

    private Quaternion returningToTrackRotationBegin;
    private Vector3 returningToTrackPositionBegin;
    private Quaternion returningToTrackRotationEnd;
    private Vector3 returningToTrackPositionEnd;

    private PlayerInputDTO player_inputs;

    /* Related to player appearance */
    private float baseIntensity = 0.6f;
    private float maxIntensity = 2f;
    private Light playerTrail;
    private MeshRenderer shipRenderer;
    private Quaternion base_ship_rotation;

    protected override void _awake()
    {
        player_health = starting_health;
        player_inputs = new PlayerInputDTO();
        playersToAttack = new Dictionary<string, RacePlayer>();

        gameEventsUI = AppConfig.findOnly<GameEventsUI>();
        placementManager = AppConfig.findOnly<PlacementManager>();

        electricalEffect = transform.GetComponentInChildren<ElectricalEffect>();
        shipRenderer = transform.Find("Ship").gameObject.GetComponent<MeshRenderer>();
        if (shipRenderer == null)
        {
            Debug.LogError("Please name the ship prefab 'Ship' in this instance of RacePlayer.cs");
        }

        //Set up ship renderer properties
        base_ship_rotation = shipRenderer.transform.localRotation;
        shipMaterial = shipRenderer.material;
        shipMaterial.SetColor("_Tint", baseTint);      
        redMaterial = new Material(Shader.Find("Transparent/Diffuse"));
        redMaterial.color = new Color32(1, 0, 0, 1);

        //TODO: do this for all subobjects so we don't have to manually set them and potentially mix them up between racers
        playerTrail = shipRenderer.transform.Find("Light").GetComponent<Light>();
        CameraPath = transform.Find("CameraPath").GetComponent<BezierSpline>();
        explosion = transform.Find("explosion").GetComponent<Explosion>();
        swipeTrail = shipRenderer.transform.Find("AttackTrail").GetComponent<SwipeTrail>();
        electricalEffect = transform.Find("ElectricEffect").GetComponent<ElectricalEffect>();

        tilt = Quaternion.identity;

        if (Physics.Raycast(transform.position, -transform.up, out downHit, rayCastDistance, AppConfig.groundMask))
        { 
            transform.position = downHit.point + hover_height * transform.up;
            transform.rotation = Quaternion.FromToRotation(transform.up, downHit.normal) * transform.rotation;
            global_orientation = transform.rotation;
            previousGravity = -downHit.normal;

            // set last checkpoint on startup in case we immediately die
            lastCheckPoint = placementManager.firstCheckPoint_A;
            lastCheckPointUp = transform.up;
            lastCheckPointPosition = transform.position + (AppConfig.hoverHeight) * lastCheckPointUp;
        } else
        {
            Debug.LogError(name + " not above track!");
        }
    }

    //TODO: refactor this into a few private functions
    protected override void _update()
    {
        if (cameraIsShaking)
        {
            shakeCamera();
        }

        if (dead)
        {
            if (pauseInvariantTime - timeStartDeath > timeAllowedDead)//&& !isEffectiveAI to keep AI dead
            {
                dead = false;
                shipRenderer.enabled = true;

                player_health = starting_health;
                shipMaterial.SetFloat("_Blend", 0);
                
                return_to_track();
            }
            return;
        }
        else if (!isEffectiveAI)
        {
            player_inputs.setFromUser();
        }

        setColorForHealth();

        setLightColor();

        //Player is returning to the track. Block other behavior by returning
        if (status == PlayerStatus.RETURNINGTOTRACK)
        {
            float deltaTime = (pauseInvariantTime - timeStartReturning) * returningToTrackSpeed;
            transform.position = Vector3.Lerp(returningToTrackPositionBegin, returningToTrackPositionEnd, deltaTime);
            transform.rotation = Quaternion.Lerp(returningToTrackRotationBegin, returningToTrackRotationEnd, deltaTime);
            if((pauseInvariantTime - timeStartReturning) >= timeSpentReturning)
            {
                status = PlayerStatus.ONTRACK;
            }
            return;
        }

        checkMiscMovement();

        bool accelerating = isEffectiveAI || player_inputs.forwardButton;

        prev_up = transform.up;


        //TODO:refactor into function
        if (Physics.Raycast(transform.position + height_above_cast * prev_up, -prev_up, out downHit, 
            inFreefall ? freeFallRayCastDistance : rayCastDistance, AppConfig.groundMask))
        {
            if(status == PlayerStatus.INAIR)
            {
                bumpSound.Play();
            }

            status = PlayerStatus.ONTRACK;
            downward_speed = 0f;

            if (accelerating)
            {
                current_speed += (current_speed >= fwd_max_speed) ? 
                    (
                        ((current_speed == fwd_max_speed) && !finishedWithRace ) ? 0 : -fwd_boost_decel
                    ) : fwd_accel * Time.deltaTime;
            }
            else if (current_speed > 0)
            {
                current_speed -= brake_speed * Time.deltaTime;
                current_speed = Math.Max(current_speed, 0f);
            }
            else
            {
                current_speed = 0f;
            }

            turnShip();         

            Vector3 desired_up = Vector3.Lerp(prev_up, downHit.normal, Time.deltaTime * pitch_smooth);
            tilt.SetLookRotation(transform.forward - Vector3.Project(transform.forward, desired_up), desired_up);
            transform.rotation =  tilt * global_orientation;

            previousGravity = -downHit.normal;

            checkGroundMovement();

            //Smoothly adjust our height
            float distance = downHit.distance - height_above_cast;
            smooth_y = Mathf.Lerp(smooth_y, hover_height - distance, Time.deltaTime * height_smooth);
            smooth_y = Mathf.Max(distance / -3, smooth_y); //sanity check on smooth_y

            transform.localPosition += prev_up * smooth_y;
            transform.position += transform.forward * (current_speed * Time.deltaTime);

        }
        /* Player is not above the track. Handle possibilities here*/
        else
        {
            /* only set this on the first frame that there is a miss */
            if (status == PlayerStatus.ONTRACK)
            {
                status = PlayerStatus.INAIR;
                if (!inFreefall)
                {
                    lastTimeOnGround = pauseInvariantTime;
                }
                wall_bounce_velocity = Vector3.zero;
            }
            /* called once to return player to the track*/
            else if ((pauseInvariantTime - lastTimeOnGround) > timeAllowedNotOnTrack && !inFreefall)
            {
                return_to_track();
            }
            /* Player moving in air */
            else
            {
                turnShip();

                transform.rotation = tilt * global_orientation ;
                if (totalPitch != 0f)
                {
                    transform.localRotation *= Quaternion.AngleAxis(totalPitch, transform.forward);
                }
                transform.position += (gravity * Time.deltaTime * Time.deltaTime + downward_speed) * previousGravity +
                    (transform.forward * (current_speed * air_speed_damping * Time.deltaTime));
                downward_speed += 0.03f;
                downward_speed = Mathf.Min(10f, downward_speed);
            }
        }
    }


    private void setLightColor()
    {
        if(current_speed <= fwd_max_speed)
        {
            playerTrail.intensity = (baseIntensity * current_speed) / fwd_max_speed;
        } else
        {
            playerTrail.intensity = maxIntensity * (current_speed - fwd_max_speed) / (fwd_boost_speed - fwd_max_speed) + baseIntensity;
        }
    }

    private void checkGroundMovement()
    {
        //we are over a headling area
        if (downHit.collider.gameObject.tag == "HealingArea")
        {
            over_healing_area = true;
            if (player_health < starting_health)
            {
                player_health = Mathf.Min(starting_health, player_health + health_per_frame_healing);
            }
        }
        else
        {
            over_healing_area = false;
            if (player_health > health_warning_thresh)
            {
                shipMaterial.SetFloat("_Blend", 0);
            }
        }

        //if there is a pitch slowly change it back to normal
        if (totalPitch != 0f)
        {
            totalPitch -= pitch_decel;
            totalPitch = Mathf.Max(totalPitch, 0);
            transform.localRotation *= Quaternion.AngleAxis(totalPitch, transform.forward);
        }

        //if there is a roll slowly change it back to normal
        if (totalRoll > 0f)
        {
            totalRoll -= roll_decel;
            totalRoll = Mathf.Max(totalRoll, 0);
        }
        else if (totalRoll < 0f)
        {
            totalRoll += roll_decel;
            totalRoll = Mathf.Min(totalRoll, 0);
        }

        //Attack the opponent racer
        if (player_inputs.attackButton)
        {
            attack_player();
        }
    }

    private void checkMiscMovement()
    {
        //Player is ricocheting of the wall
        if (!Vector3.zero.Equals(wall_bounce_velocity))
        {
            transform.position += wall_bounce_velocity;
            wall_bounce_velocity /= wall_bounce_deccel;

            if (wall_bounce_velocity.magnitude < wall_bounce_threshold)
            {
                wall_bounce_velocity = Vector3.zero;
            }
        }

        //Player is attacking another
        if (!Vector3.zero.Equals(attack_velocity))
        {
            transform.position += attack_velocity;
            attack_velocity /= attack_deccel;

            shipRenderer.transform.localRotation = Quaternion.AngleAxis(50f, shipRenderer.transform.forward) * base_ship_rotation;


            if (Mathf.Abs(attack_velocity.sqrMagnitude) < attack_threshold)
            {
                attack_velocity = Vector3.zero;
            }
        }

        //Player was attacked
        if (!Vector3.zero.Equals(attacked_velocity))
        {
            transform.position += attacked_velocity;
            attacked_velocity /= attack_deccel;

            if (Mathf.Abs(attacked_velocity.sqrMagnitude) < attack_threshold)
            {
                attacked_velocity = Vector3.zero;
            }
        }

        //Player is using boost power
        if (player_inputs.boostButton && (pauseInvariantTime - lastTimeBoostPower > boost_time_window))
        {
            if(player_health <= 2f) return;

            float boost_factor = 1f ;//want to use less than normal boost power if low health
            if(player_health <= boost_cost + 1)
            {
                boost_factor = player_health / (boost_cost + 1);
            }

            current_speed += boost_factor * (fwd_boost_speed - current_speed);
            boostEffects();
            lastTimeBoostPower = pauseInvariantTime;
            slowlyDamage((int)(boost_factor * boost_cost));
        }

        if (!isAI && current_speed >= fwd_max_speed)
        {
            Camera.main.transform.localPosition = playerToCamera;
        }
    }

    private void shakeCamera()
    {
        //don't shake the camera during the finish line sequence
        if (finishedWithRace)
        {
            return;
        }

        float randNrX = UnityEngine.Random.Range(gyrationFactor, -gyrationFactor);
        float randNrY = UnityEngine.Random.Range(gyrationFactor, -gyrationFactor);
        float randNrZ = UnityEngine.Random.Range(gyrationFactor, -gyrationFactor);
        Camera.main.transform.position += new Vector3(randNrX, randNrY, randNrZ);
    }

    private void shakeCameraForSeconds(float timeCameraShakeBump)
    {
        if (isEffectiveAI) return;

        cameraIsShaking = true;
        callAfterSeconds(timeCameraShakeBump, () =>
        {
            cameraIsShaking = false;
            Camera.main.transform.localPosition = playerToCamera;
        });
    }

    private void return_to_track()
    {
        status = PlayerStatus.RETURNINGTOTRACK;
        timeStartReturning = pauseInvariantTime;
        attack_velocity = Vector3.zero;
        attacked_velocity = Vector3.zero;
        wall_bounce_velocity = Vector3.zero;

        current_speed = 0f;
        totalPitch = 0f;
        totalRoll = 0f;

        returningToTrackRotationBegin = transform.rotation;
        returningToTrackPositionBegin = transform.position;
        returningToTrackRotationEnd = Quaternion.LookRotation(lastCheckPoint.tangent, lastCheckPointUp);
        returningToTrackPositionEnd = lastCheckPointPosition;
    }

    private void setColorForHealth()
    {
        //TODO:add beeping too?
        if (over_healing_area)
        {
            float t = Mathf.PingPong(Time.time * 1f, 0.4f);
            shipMaterial.SetColor("_Tint", tintColor);
            shipMaterial.SetFloat("_Blend", t);
        }
        else
        {
            shipMaterial.SetColor("_Tint", baseTint);
            if (player_health < health_warning_thresh)
            {
                float t = Mathf.PingPong(Time.time * health_blink_speed / player_health, 0.4f);
                shipMaterial.SetFloat("_Blend", t);
            }
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        switch (coll.gameObject.tag)
        {

            //create a velocity vector from bouncing off the wall
            case "CollissionWall":

                if (Physics.Raycast(transform.position, -transform.right, out wallHit, rayCastDistance, AppConfig.wallMask))
                {
                    current_speed /= wall_bounce_curr_speed_deccel;
                }
                else if (Physics.Raycast(transform.position, transform.right, out wallHit, rayCastDistance, AppConfig.wallMask))
                {
                    current_speed /= wall_bounce_curr_speed_deccel;
                }
                else if (Physics.Raycast(transform.position - 2f * transform.forward, transform.forward, out wallHit, rayCastDistance, AppConfig.wallMask))
                {
                    current_speed = 0f;
                } else
                {
                    Debug.LogWarning("Detected hit with wall but cannot find wall on left, right or front!");
                    return;
                }

                shakeCameraForSeconds(timeCameraShakeBump);
                damage(attack_bump_damage);
                bumpSound.Play();

                wall_bounce_velocity = wallHit.normal * wall_bounce_speed_to_bounce_ratio * current_speed;

                break;

            //when we hit a boost panel
            case "BoostPanel":
                current_speed = fwd_boost_speed;
                boostEffects();
                coll.gameObject.GetComponent<BoostPanel>().boostAnimation();
                break;

            //when we cross the finish line
            case "FinishLine":
                if (status == PlayerStatus.RETURNINGTOTRACK) return;

                startedLap1 = true;
                placementManager.crossFinish(this);
                break;

            //when we hit a trackpoint trigger
            case "TrackPoint":
                if (status != PlayerStatus.RETURNINGTOTRACK)
                {
                    TrackPoint trackPoint = coll.gameObject.GetComponent<TrackPoint>();

                    //AI should only stick to one path
                    if (!isEffectiveAI || AIPathChoice == trackPoint.pathChoice)
                    {
                        current_TrackPoint = trackPoint;
                    }
                    
                    if(placementManager.updateTrackPoint(this, trackPoint)){
                        lastCheckPoint = trackPoint;
                        lastCheckPointUp = transform.up;
                        lastCheckPointPosition = transform.position + (AppConfig.hoverHeight) * lastCheckPointUp;
                    }
                }
                break;

            //attack or bump other player
            case "Player":
                if (!playersToAttack.ContainsKey(coll.name))
                {
                    playersToAttack.Add(coll.name, coll.gameObject.GetComponent<RacePlayer>());
                }
                bump(coll.gameObject.GetComponent<RacePlayer>(), false);

                //the AI may decide to attack another player >:)
                //TODO: I should add more special effects for this. It's not obvious they're attacking
                if (isAI)
                {
                    callAfterSeconds(0.1f, () =>
                    {
                        attack_player();
                    });
                }
                break;

            //Make sure falling player doesn't fall through ground
            case "Ground":            
                if(status == PlayerStatus.INAIR || inFreefall)
                {
                    if (Physics.Raycast(transform.position -5 * transform.forward, transform.forward, out downHit, 15, AppConfig.groundMask))
                    {
                        //TODO:test more, but pretty good!
                        float rho = Vector3.Dot(transform.forward, downHit.normal);
                        status = PlayerStatus.ONTRACK;
                        transform.position = downHit.point + downHit.normal * hover_height;
                        current_speed *= 1f / (1 + 3 * rho * rho);
                        downward_speed = 0f;
                    }
                }
                break;
            case "DeathZone":
                if(status == PlayerStatus.INAIR)
                {
                    return_to_track();
                }
                break;

            //Log warning for unhandled tag
            default:
                Debug.LogWarning("No behavior for OnTriggerEnter with tag: " + coll.gameObject.tag);
                break;
        }
    }

    void OnTriggerStay(Collider coll)
    {
        switch (coll.gameObject.tag)
        {
            case "FreeFallZone":
                inFreefall = true;
                break;

            //don't do anything for most tags
            default:
                break;
        }
    }

    void OnTriggerExit(Collider coll)
    {
        switch (coll.gameObject.tag)
        {
            case "FreeFallZone":
                inFreefall = false;
                if (status == PlayerStatus.INAIR)
                {
                    lastTimeOnGround = pauseInvariantTime;
                }
                break;
            case "Player":
                callAfterSeconds(0.4f, () => {
                    if(playersToAttack.ContainsKey(coll.name))
                        playersToAttack.Remove(coll.name);
                });
                break;

            //don't do anything for most tags
            default:
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

    private void turnShip()
    {
        //Find horizonal input 
        float horizontal_input;
        float vertical_input;
        bool spaceBar = false;
        if (!isEffectiveAI || dead)
        {
            horizontal_input = player_inputs.horizonalAxis;
            vertical_input = player_inputs.verticalAxis;
            spaceBar = player_inputs.sharpTurnButton;
            prev_h = horizontal_input;
        }
        else
        {
            
            if (current_TrackPoint == null)
            {
                horizontal_input = player_inputs.horizonalAxis;
                vertical_input = player_inputs.verticalAxis;
            }
            else
            {
                setInputsFromAI(out horizontal_input, out vertical_input, out spaceBar);
            }
        }
   
        //calculate turn angle
        float turn_angle = 0f;
        if (status == PlayerStatus.INAIR)
        {
            turn_angle = air_turn_speed * horizontal_input;

            //calculate vertical axis for nose diving
            if (vertical_input < 0)
            {
                totalPitch = Mathf.Min(totalPitch + pitch_per_vert, max_pitch);
            }
            else if (vertical_input > 0)
            {
                totalPitch = Mathf.Max(totalPitch - pitch_per_vert, min_pitch);
            }
        }
        else
        {
            turn_angle = turn_speed * Time.deltaTime * horizontal_input;

            if (spaceBar)
            {
                turn_angle *= hard_turn_multiplier;
            }

            if (totalRoll != 0)
            {
                shipRenderer.transform.localRotation = Quaternion.AngleAxis(totalRoll, shipRenderer.transform.up) * base_ship_rotation;
            }
            else
            {
                shipRenderer.transform.localRotation = Quaternion.AngleAxis((spaceBar ? ship_mesh_tilt_hard_turn : ship_mesh_tilt) * turn_angle, shipRenderer.transform.forward) * base_ship_rotation;
            }
        }

        global_orientation = Quaternion.Euler(0, turn_angle, 0);
    }

    private void setInputsFromAI(out float horizontal_input, out float vertical_input, out bool spaceBar)
    {
        // for h - is turning left and + is right!
        spaceBar = false;
        vertical_input = 0f;
        int nearEdge = AIUtil.checkIfNearEdge(transform.position, transform.up, current_TrackPoint);

        // Need to turn the ship away from the edge
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
            horizontal_input = Mathf.Clamp(horizontal_input, -1, 1);
            prev_h = horizontal_input;
            return;
        }

        // Look a few trackpoints ahead if we are on a track with many sharp turns.
        // TODO: bug, somehow current_TrackPoint can be set to the stupid "Initial TrackPoint" if it exists
        float h3 = AIUtil.getHorizontal(transform.position, transform.forward, current_speed, current_TrackPoint.next.next.next);
        float h2 = AIUtil.getHorizontal(transform.position, transform.forward, current_speed, current_TrackPoint.next.next);
        float h1 = AIUtil.getHorizontal(transform.position, transform.forward, current_speed, current_TrackPoint.next);
        horizontal_input = 0.25f * h1 + 0.25f * h2 + 0.5f * h3;

        if (horizontal_input > hard_turn_multiplier)
        {
            spaceBar = true;
        }
        horizontal_input = Mathf.Clamp01(horizontal_input);

        float signDiff = Vector3.Dot(Vector3.Cross(transform.forward, transform.up), current_TrackPoint.tangent);
        int sign = signDiff > 0 ? -1 : 1;

        //Vector3.Dot(transform.position - current_TrackPoint.transform.position, )

        horizontal_input *= sign;

        if(status == PlayerStatus.INAIR)
        {
            vertical_input = prev_v + max_delta_v;
            vertical_input = vertical_input > 1 ? 1 : vertical_input;
        }
        else if(prev_v != 0)
        {
            vertical_input = prev_v - max_delta_v;
            vertical_input = vertical_input < 0 ? 0 : vertical_input;
        }

        if (horizontal_input - prev_h >= max_delta_h)
            horizontal_input = prev_h + max_delta_h;
        else if (prev_h - horizontal_input >= max_delta_h)
            horizontal_input = prev_h - max_delta_h;

        horizontal_input = Mathf.Clamp(horizontal_input, -1, 1);

        prev_h = horizontal_input;
        prev_v = vertical_input;

    }

    private void boostEffects()
    {
        boostSound.Play();
        electricalEffect.Activate();
    }

    /**
     * this player decides to attack an opponent. The opponent is chosen here from those possible
     */
    private void attack_player()
    {
        if(pauseInvariantTime - lastTimeAttacked > attack_time_window && playersToAttack.Count > 0)
        {
            lastTimeAttacked = pauseInvariantTime;

            var enumerator = playersToAttack.GetEnumerator();
            enumerator.MoveNext();
            RacePlayer opponent = playersToAttack[enumerator.Current.Key];
            playersToAttack.Remove(enumerator.Current.Key);

            Debug.Log(name + " Attacked " + opponent);

            bump(opponent, true);
        }       
    }

    /**
     * Bumps opponent raceplayer out of way. attacking is true when this player is attacking
     * and false when this is only meant to represent 2 players colliding
     */
    private void bump(RacePlayer opponent, bool attacking)
    {
        Vector3 playerToOpponent = (opponent.transform.position - transform.position).normalized;
        attack_velocity = playerToOpponent * (attacking ? attacked_magnitude : attack_bump_magnitude);
        float _damage = attacking ? attack_damage_multiplier * current_speed : attack_bump_damage;

        if (attacking)
        {
            if (swipeTrail != null)
            {
                swipeTrail.swipe(Vector3.Dot(playerToOpponent, transform.right) < 0);
            }
            damage(attack_damage_transfer_factor * _damage);
            totalRoll = Vector3.Dot(playerToOpponent, transform.right) > 0 ? attack_roll : -attack_roll;
        }
        else
        {
            bumpSound.Play();
        }

        opponent.attack(name, playerToOpponent, _damage, attacking);
    }

    /**
     * called when one player attacks another from _dir direction with _damage
     */
    public void attack(string attackerName, Vector3 _dir, float _damage, bool _attacking)
    {
        attacked_velocity = _dir;
        if (_attacking)
        {
            totalRoll = Vector3.Dot(_dir, transform.right) < 0 ? attack_roll : -attack_roll;
        }

        // attack killed the this player
        if (!dead && ((player_health - _damage) <= 0))
        {
            gameEventsUI.PlayerKilledMessage(name, attackerName);
        }
        damage(_damage);
    }

    /**
     * called to slowly inflict damage (10 points per second) to the player by _damage amount
     */
    public void slowlyDamage(int _damage)
    {
        while(_damage > 0)
        {
            callAfterSeconds(0.1f * _damage, () =>
            {
                damage(1f);
            });
            --_damage;
        }
    }

    /**
     * called to damage the current player by _damage amount
     */
    public void damage(float _damage)
    {
        if (dead) return;

        player_health -= _damage;
        player_health = Math.Max(player_health, 0f);
        player_health = Math.Min(player_health, max_bonus_health);

        //The Player has died >:)
        if (player_health <= 0)
        {
            gameEventsUI.PlayerDeathMessage(name);
            dead = true;
            timeStartDeath = pauseInvariantTime;
            explosion.Trigger(transform.position, transform.rotation, timeCameraShake);
            current_speed = 0;
            shipRenderer.enabled = false;
            shakeCameraForSeconds(timeCameraShake);
        }
    }

    /**
     * called when the player finishes the race. To change it's behavior
     */
    public void finishRace()
    {
        if (finishedWithRace) return;

        gameEventsUI.PlayerFinishMessage(name);
        finishedWithRace = true;
        fwd_max_speed *= 0.6f;
    }

    public void finishMainPlayer()
    {
        finishRace();
        Camera.main.transform.localPosition = finishedCameraPosition;
        Camera.main.transform.localRotation = finishedCameraRotation;
    }
}
