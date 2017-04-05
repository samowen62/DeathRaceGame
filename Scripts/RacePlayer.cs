using UnityEngine;
using System.Collections.Generic;
using System;

/**
 * NOTES:
 * 
 *  1) To set the initial position and rotation of this object in unity let this player start for
 *     just a second or two and use the resulting orientation so that we take rigidBody motion into account
 */
public class RacePlayer : PausableBehaviour
{

    /* AI indicator */
    public bool isAI = true;
    private int _placement;
    public int placement
    {
        get
        {
            return _placement;
        }
        set{
            if (!finished)
            {
                _placement = value;
            }
        }
    }

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
    private float yaw;
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

    private float wall_bounce_velocity = 0f;
    private PlayerStatus status = PlayerStatus.ONTRACK;

    /* Related to player gliding */
    private bool inFreefall;
    private float totalPitch;
    public float max_pitch = 60;
    public float min_pitch = -30;
    public float pitch_per_vert = 5f;
    public float air_speed_damping = 0.6f;
    public float pitch_decel = 10f;

    /* Related to player attacking */
    public Explosion explosion;
    private Dictionary<string, RacePlayer> playersToAttack;
    private Vector3 attack_velocity = Vector3.zero;
    private Vector3 attacked_velocity = Vector3.zero;
    public AudioObject bumpSound;
    public float attack_time_window = 0.4f;
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
    private float timeAllowedDead = 4f;
    private float gyrationFactor = 0.2f;

    /* Related to player health */
    private Material shipMaterial;
    private Material redMaterial;
    public float starting_health = 100f;
    public float max_bonus_health = 150f;
    public float health_per_frame_healing = 0.2f;
    public float health_blink_speed = 20f;
    private float health_warning_thresh = 25f;
    private float player_health;
    public float health
    {
        get
        {
            return player_health;
        }
    }

    /* Last checkpoint of the player */
    private CheckPoint lastCheckPoint;
    private TrackPoint current_TrackPoint;
    public int trackPointNumInSeq
    {
        get
        {
            if(current_TrackPoint != null)
            {
                return current_TrackPoint.num_in_seq;
            }
            else
            {
                return -1;
            }
        }
    }


    public bool passedFinish = false;
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
    private float timeStartReturning = 0f;
    private float timeStartDeath = 0f;

    private Vector3 playerToCamera = new Vector3(0, 10, -20);

    private Quaternion returningToTrackRotationBegin;
    private Vector3 returningToTrackPositionBegin;
    private Quaternion returningToTrackRotationEnd;
    private Vector3 returningToTrackPositionEnd;

    private PlayerInputDTO player_inputs;

    //renderer of ship
    private MeshRenderer shipRenderer;
    private Quaternion base_ship_rotation;

    protected override void _awake()
    {
        player_health = starting_health;
        player_inputs = new PlayerInputDTO();
        playersToAttack = new Dictionary<string, RacePlayer>();

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
        shipMaterial = shipRenderer.material;
        redMaterial = new Material(Shader.Find("Transparent/Diffuse"));
        redMaterial.color = new Color32(1, 0, 0, 1);
        tilt = Quaternion.identity;

        lastCheckPoint = track[0].startingCheckPoint;

        //TODO: Fix this to avoid jumping the rotation on start. Maybe just give current speed on start?
        if (Physics.Raycast(transform.position, -transform.up, out downHit, rayCastDistance, AppConfig.groundMask))
        { 
            transform.position = downHit.point + hover_height * transform.up;
            transform.rotation = Quaternion.FromToRotation(transform.up, downHit.normal) * transform.rotation;
            global_orientation = transform.rotation;
            yaw = transform.rotation.eulerAngles.y;
            previousGravity = -downHit.normal;
        } else
        {
            Debug.LogError(name + " not above track!");
        }
    }

    //TODO: refactor this into a few private functions
    protected override void _update()
    {
        if (dead)
        {
            if(pauseInvariantTime - timeStartDeath < timeCameraShake && !isEffectiveAI)
            {
                shakeCamera();
            }
            if (pauseInvariantTime - timeStartDeath > timeAllowedDead && !isEffectiveAI)
            {
                Debug.Log("returning" + (pauseInvariantTime - timeStartDeath));
                dead = false;
                shipRenderer.enabled = true;

                player_health = starting_health;
                shipRenderer.material.SetFloat("_Blend", 0);

                //TODO:this needs to be fixed
                Camera.main.transform.localPosition = playerToCamera;
                return_to_track();
            }
            return;
        }
        else if (!isEffectiveAI)
        {
            player_inputs.setFromUser();
        }

        setColorForHealth();
    
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

        //Player is ricocheting of the wall
        if(wall_bounce_velocity != 0)
        {
            transform.position += wall_bounce_velocity * transform.right;
            wall_bounce_velocity /= wall_bounce_deccel;

            if(Mathf.Abs(wall_bounce_velocity) < wall_bounce_threshold)
            {
                wall_bounce_velocity = 0;
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
            //Debug.Log("attacked :" + attacked_velocity);
            transform.position += attacked_velocity;
            attacked_velocity /= attack_deccel;

            if (Mathf.Abs(attacked_velocity.sqrMagnitude) < attack_threshold)
            {
                attacked_velocity = Vector3.zero;
            }
        }

        bool accelerating = isEffectiveAI || player_inputs.w_key;

        prev_up = transform.up;

        /* Adjust the position and rotation of the ship to the track */
        //Debug.DrawRay(transform.position + height_above_cast * prev_up, -prev_up, Color.red, 120);

        if (Physics.Raycast(transform.position + height_above_cast * prev_up, -prev_up, out downHit, 
            inFreefall ? freeFallRayCastDistance : rayCastDistance, AppConfig.groundMask))
        {
            if(status == PlayerStatus.INAIR)
            {
                bumpSound.Play();
            }

            status = PlayerStatus.ONTRACK;
            downward_speed = 0f;

            if (downHit.collider.gameObject.tag == "HealingArea")
            {
                
                if(player_health < starting_health)
                {
                    player_health = Mathf.Min(starting_health, player_health + health_per_frame_healing);
                }

                if(player_health > health_warning_thresh)
                {
                    shipRenderer.material.SetFloat("_Blend", 0);
                }
            }

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

            turnShip(false);         

            Vector3 desired_up = Vector3.Lerp(prev_up, downHit.normal, Time.deltaTime * pitch_smooth);
            tilt.SetLookRotation(transform.forward - Vector3.Project(transform.forward, desired_up), desired_up);
            transform.rotation =  tilt * global_orientation;

            previousGravity = -downHit.normal;

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
            if (player_inputs.e_key && playersToAttack.Count > 0)
            {
                attack_player();
            }

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
                wall_bounce_velocity = 0;
            }
            /* called once to return player to the track*/
            else if ((pauseInvariantTime - lastTimeOnGround) > timeAllowedNotOnTrack && !inFreefall)
            {
                return_to_track();
            }
            /* Player moving in air */
            else
            {
                turnShip(true);

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

    private void shakeCamera()
    {
        float randNrX = UnityEngine.Random.Range(gyrationFactor, -gyrationFactor);
        float randNrY = UnityEngine.Random.Range(gyrationFactor, -gyrationFactor);
        float randNrZ = UnityEngine.Random.Range(gyrationFactor, -gyrationFactor);
        Camera.main.transform.position += new Vector3(randNrX, randNrY, randNrZ);
    }

    private void return_to_track()
    {
        status = PlayerStatus.RETURNINGTOTRACK;
        timeStartReturning = pauseInvariantTime;
        current_speed = 0f;
        yaw = lastCheckPoint.yaw;

        returningToTrackRotationBegin = transform.rotation;
        returningToTrackPositionBegin = transform.position;
        returningToTrackRotationEnd = Quaternion.LookRotation(lastCheckPoint.trackForward, lastCheckPoint.trackNormal);
        returningToTrackPositionEnd = lastCheckPoint.trackPoint;
    }

    private void setColorForHealth()
    {
        //add beeping too?
        if (player_health < health_warning_thresh)
        {
            float t = Mathf.PingPong(Time.time * health_blink_speed / player_health, 0.4f);
            shipRenderer.material.SetFloat("_Blend", t);
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
                    damage(attack_bump_damage);
                    bumpSound.Play();

                    wall_bounce_velocity = -wall_bounce_speed_to_bounce_ratio * current_speed * Vector3.Dot(wallHit.normal, transform.forward);
                    current_speed /= wall_bounce_curr_speed_deccel;
                }
                else if (Physics.Raycast(transform.position, transform.right, out wallHit, rayCastDistance, AppConfig.wallMask))
                {
                    damage(attack_bump_damage);
                    bumpSound.Play();

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
                boostSound.Play();
                coll.gameObject.GetComponent<BoostPanel>().boostAnimation();
                break;

            //when we cross the finish line
            case "FinishLine":
                passedFinish = true;
                break;

            //when we hit a trackpoint trigger
            case "TrackPoint":
                if (status != PlayerStatus.RETURNINGTOTRACK)
                {
                    current_TrackPoint = coll.gameObject.GetComponent<TrackPoint>();
                }
                break;

            //attack or bump other player
            case "Player":
                if (!playersToAttack.ContainsKey(coll.name))
                {
                    playersToAttack.Add(coll.name, coll.gameObject.GetComponent<RacePlayer>());
                }
                bump(coll.gameObject.GetComponent<RacePlayer>(), false);
                break;

            //Make sure falling player doesn't fall through ground
            case "Ground":
                if(status == PlayerStatus.INAIR || inFreefall)
                {
                    Debug.DrawLine(transform.position - 5 * transform.forward, transform.position + 15 * transform.forward, Color.red);
                    if (Physics.Raycast(transform.position -5 * transform.forward, transform.forward, out downHit, 15, AppConfig.groundMask))
                    {
                        Debug.Log("hit ground " + downHit.distance);
                        //TODO:fix this!!
                        Debug.Log(transform.position -( downHit.normal * hover_height));
                        //transform.position = downHit.normal * hover_height;
                        //totalPitch = 0;
                    }
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

    /**
     * returns how deep into the trackpoint this player is
     */
    public float depthInTrackPoint()
    {
        if (current_TrackPoint != null)
        {
            return current_TrackPoint.distanceTraversed(transform.position);
        }
        else
        {
            return 0f;
        }
    }

    private void turnShip(bool inAir)
    {
        //Find horizonal input 
        float horizontal_input;
        float vertical_input;
        bool spaceBar = false;
        if (!isEffectiveAI || dead)
        {
            horizontal_input = player_inputs.horizonalAxis;
            vertical_input = player_inputs.verticalAxis;
            spaceBar = player_inputs.spaceBar;
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
        if (inAir)
        {
            turn_angle = air_turn_speed * horizontal_input;
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

        yaw += turn_angle;

        //calculate vertical axis for nose diving
        if (status == PlayerStatus.INAIR)
        {
            if (vertical_input > 0)
            {
                totalPitch = Mathf.Min(totalPitch + pitch_per_vert, max_pitch);
            }
            else if (vertical_input < 0)
            {
                totalPitch = Mathf.Max(totalPitch - pitch_per_vert, min_pitch);
            }
        }

        global_orientation = Quaternion.Euler(0, turn_angle, 0);
    }

    private void setInputsFromAI(out float horizontal_input, out float vertical_input, out bool spaceBar)
    {
        //for h - is turning left and + is right!
        spaceBar = false;
        vertical_input = 0f;
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

        //Debug.Log(" h:" + horizontal_input + " prev_h:" + prev_h + " pitch: " + totalPitch +  " sign:" + sign);

        if(status == PlayerStatus.INAIR)
        {
            vertical_input = prev_v + max_delta_v;
            vertical_input = vertical_input > 1 ? 1 : vertical_input;
        }else if(prev_v != 0)
        {
            vertical_input = prev_v - max_delta_v;
            vertical_input = vertical_input < 0 ? 0 : vertical_input;
        }

        if (horizontal_input - prev_h >= max_delta_h)
            horizontal_input = prev_h + max_delta_h;
        else if (prev_h - horizontal_input >= max_delta_h)
            horizontal_input = prev_h - max_delta_h;

        //TODO inline function for clamping between +/- 1
        horizontal_input = horizontal_input > 1 ? 1 : horizontal_input;
        horizontal_input = horizontal_input < -1 ? -1 : horizontal_input;

        prev_h = horizontal_input;
        prev_v = vertical_input;

    }

    /**
     * this player decides to attack an opponent. The opponent is chosen here from those possible
     */
    private void attack_player()
    {
        if(pauseInvariantTime - lastTimeAttacked > attack_time_window)
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
            damage(attack_damage_transfer_factor * _damage);
            totalRoll = Vector3.Dot(playerToOpponent, transform.right) > 0 ? attack_roll : -attack_roll;
        }
        else
        {
            bumpSound.Play();
        }

        opponent.attack(playerToOpponent, _damage, attacking);
    }

    /**
     * called when one player attacks another from _dir direction with _damage
     */
    public void attack(Vector3 _dir, float _damage, bool _attacking)
    {
        attacked_velocity = _dir;
        if (_attacking)
        {
            totalRoll = Vector3.Dot(_dir, transform.right) < 0 ? attack_roll : -attack_roll;
        }
        damage(_damage);
    }

    /**
     * called to damage the current player by _damage amount
     */
    public void damage(float _damage)
    {
        player_health -= _damage;
        player_health = Math.Max(player_health, 0f);
        player_health = Math.Min(player_health, max_bonus_health);

        //The Player has died
        if (player_health <= 0)
        {
            Debug.Log(name + " died!");
            dead = true;
            timeStartDeath = pauseInvariantTime;
            explosion.Trigger(transform.position, transform.rotation, timeCameraShake);
            current_speed = 0;
            shipRenderer.enabled = false;
        }
    }

    /**
     * called when the player finishes the race. To change it's behavior
     */
    public void finishRace()
    {
        if (finishedWithRace) return;

        finishedWithRace = true;
        fwd_max_speed *= 0.6f;
        Camera.main.transform.localPosition = finishedCameraPosition;
        Camera.main.transform.localRotation = finishedCameraRotation;
    }
}
