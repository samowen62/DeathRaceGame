﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class RacePlayer : PausableBehaviour
{
    #region Enums
    public enum Handling { Versatile, Normal, Poor }
    public enum Speed { HighTopLowAcc, Normal, LowTopHighAcc }
    public enum Power { High, Normal, Low }
    public enum Health { Resilient, Normal, Weak }
    public enum AIDifficulty { Hard, Normal, Easy }

    [SerializeField]
    private Handling _handlingGrade = Handling.Normal;
    public Handling HandlingGrade
    {
        get
        {
            return HandlingGrade;
        }
        set
        {
            switch (value)
            {
                case Handling.Versatile:
                    turn_speed = 87f;
                    hard_turn_multiplier = 2.3f;
                    air_turn_speed = 15f;
                    break;

                case Handling.Normal:
                    turn_speed = 80f;
                    hard_turn_multiplier = 2.2f;
                    air_turn_speed = 10f;
                    break;

                case Handling.Poor:
                    turn_speed = 68f;
                    hard_turn_multiplier = 2.0f;
                    air_turn_speed = 8f;
                    break;
            }
            HandlingGrade = value;
        }
    }

    [SerializeField]
    private Speed _speedGrade = Speed.Normal;
    public Speed SpeedGrade
    {
        get
        {
            return _speedGrade;
        }
        set
        {
            switch (value)
            {
                case Speed.HighTopLowAcc:
                    fwd_accel = 75f;
                    fwd_max_speed = 180;
                    fwd_boost_speed = 225;
                    fwd_boost_decel = 2.0f;
                    break;

                case Speed.Normal:
                    fwd_accel = 80f;
                    fwd_max_speed = 170;
                    fwd_boost_speed = 220;
                    fwd_boost_decel = 2.5f;
                    break;

                case Speed.LowTopHighAcc:
                    fwd_accel = 85f;
                    fwd_max_speed = 160;
                    fwd_boost_speed = 215;
                    fwd_boost_decel = 2.7f;
                    break;
            }
            _speedGrade = value;
        }
    }

    [SerializeField]
    private Power _powerGrade = Power.Normal;
    public Power PowerGrade
    {
        get
        {
            return _powerGrade;
        }
        set
        {
            switch (value)
            {
                case Power.High:
                    attack_damage_multiplier = 0.5f;
                    damage_damper = 0.6f;
                    break;

                case Power.Normal:
                    attack_damage_multiplier = 0.3f;
                    damage_damper = 1.0f;
                    break;

                case Power.Low:
                    attack_damage_multiplier = 0.2f;
                    damage_damper = 1.2f;
                    break;
            }
            _powerGrade = value;
        }
    }

    [SerializeField]
    private Health _healthGrade = Health.Normal;
    public Health HealtGrade
    {
        get
        {
            return _healthGrade;
        }
        set
        {
            switch (value)
            {
                case Health.Resilient:
                    starting_health = 120f;
                    max_bonus_health = 150f;
                    health_per_frame_healing = 4.0f;
                    break;

                case Health.Normal:
                    starting_health = 100f;
                    max_bonus_health = 125f;
                    health_per_frame_healing = 3.0f;
                    break;

                case Health.Weak:
                    starting_health = 85f;
                    max_bonus_health = 100f;
                    health_per_frame_healing = 2.0f;
                    break;
            }
            _healthGrade = value;
        }
    }

    [SerializeField]
    private AIDifficulty _aiDifficultyGrade = AIDifficulty.Normal;
    public AIDifficulty AIDifficultyGrade
    {
        get
        {
            return _aiDifficultyGrade;
        }
        set
        {
            switch (value)
            {
                case AIDifficulty.Hard:
                    ai_attack_allowed_inc = 3f;
                    ai_attack_allowed_cooldown = 2f;
                    break;

                case AIDifficulty.Normal:
                    ai_attack_allowed_inc = 5f;
                    ai_attack_allowed_cooldown = 4f;
                    break;

                case AIDifficulty.Easy:
                    ai_attack_allowed_inc = 10f;
                    ai_attack_allowed_cooldown = 20f;
                    break;
            }
            _aiDifficultyGrade = value;
        }
    }


    #endregion

    #region Instance Variables
    /* AI indicator */
    public bool isAI = true;
    public TrackPoint.PathChoice AIPathChoice = TrackPoint.PathChoice.PATH_A;

    /* Related to air and returning mechanics */
    private float gravity = 1700f;
    private float returningToTrackSpeed = 0.8f;
    private float timeAllowedNotOnTrack = 2f;
    private float timeSpentReturning = 1.5f;


    /* Ship handling parameters, must be multiples of 5!! */
    private float fwd_accel = 80f;
    private float fwd_max_speed = 170;
    private float fwd_boost_speed = 220;
    private float fwd_boost_decel = 2.5f;
    private float boost_time_window = 2.2f;
    private int boost_cost = 10;
    private AudioObject boostSound;
    public float MaxSpeed { get { return fwd_max_speed; } }

    private float brake_speed = 200f;
    private float turn_speed = 80f;
    private float hard_turn_multiplier = 2.2f;
    private float air_turn_speed = 10f;

    /* Related to for bouncing against the wall */
    private float wall_bounce_deccel = 10f;//must be > 1!!
    private float wall_bounce_threshold = 5f;
    private float wall_bounce_speed_to_bounce_ratio = 0.1f;
    private float wall_bounce_curr_speed_deccel = 2f;//must be > 1!!

    /* Related to for ship animation */
    private float ship_mesh_tilt = 5f;
    private float ship_mesh_tilt_hard_turn = 3f;

    /* Related to ship orientation and sticking to the track*/
    private float hover_height = AppConfig.hoverHeight - 0.5f;
    private float height_smooth = 12f;                               //How fast the ship will readjust to "hover_height"
    private float pitch_smooth = 5f;                                //How fast the ship will adjust its rotation to match track normal
    private float height_correction = 2.2f;
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
    private float pitch_per_vert = 1.2f;
    private float air_speed_damping = 0.6f;
    private float pitch_decel = 10f;

    /* Related to player effects */
    private Explosion explosion;
    private SwipeTrail swipeTrail;
    private ElectricalEffect electricalEffect;

    private Dictionary<string, RacePlayer> playersToAttack;
    private Vector3 attack_velocity = Vector3.zero;
    private Vector3 attacked_velocity = Vector3.zero;
    private AudioObject bumpSound;
    private float ai_attack_allowed_inc = 3f;
    private float ai_attack_allowed_cooldown = 3f;
    private float attack_time_window = 0.25f;
    private float attack_deccel = 10;
    private float attack_threshold = 5;
    private float attacked_magnitude = 2;
    private float attack_damage_multiplier = 0.3f;
    private float damage_damper = 1.0f;
    private float attack_bump_damage = 1f;
    private float attack_bump_magnitude = 0.1f;
    private float attack_damage_transfer_factor = -0.8f;
    private float totalRoll;
    private float roll_decel = 1.4f;
    private float attack_roll = 25f;
    private bool dead = false;
    public bool IsDead
    {
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
    private float starting_health = 100f;
    private float max_bonus_health = 125f;
    private float health_per_frame_healing = 0.2f;
    private float health_blink_speed = 20f;
    public bool cheat_infinite_boost = false;
    private bool over_healing_area = false;
    private float health_warning_thresh = 25f;
    private float player_health;
    public float StartingHealth { get { return starting_health; } }
    public float MaxBonusHealth { get { return max_bonus_health; } }
    public float health
    {
        get
        {
            return player_health;
        }
    }

    private PlacementManager placementManager;
    private GameEventsUI gameEventsUI;
    private MachineCatcher catcher;

    /* Last checkpoint of the player */
    private Vector3 lastCheckPointPosition;
    private Quaternion lastCheckPointRotation;
    private TrackPoint lastCheckPoint;
    private TrackPoint current_TrackPoint;

    public bool StartedLap1 { get; private set; }
    private bool finishedWithRace = false;
    public bool Finished
    {
        get
        {
            return finishedWithRace;
        }
    }
    private bool isEffectiveAI
    {
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

    private Vector3 _playerToCamera = new Vector3(0, 10, -20);
    public BezierSpline CameraPath { get; set; }
    public Quaternion CameraRotation { get { return Quaternion.Euler(6, 0, 0); } }
    public Vector3 PlayerToCamera
    {
        get
        {
            if (finishedWithRace)
            {
                return finishedCameraPosition;
            }
            if (current_speed <= fwd_max_speed)
            {
                return _playerToCamera;
            }
            float target_z_distance = Mathf.Lerp(0, boostPlayerToCamera_Z,
                (current_speed - fwd_max_speed) / (fwd_boost_speed - fwd_max_speed));
            //control how fast the camera can zoom out
            if (target_z_distance - boostCameraDistance > boostCameraSpeed)
            {
                boostCameraDistance += boostCameraSpeed;
            }
            else if (boostCameraDistance - target_z_distance > boostCameraSpeed)
            {
                boostCameraDistance -= boostCameraSpeed;
            }
            else
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
    #endregion

    #region PausableBehaviour Members
    protected override void _awake()
    {
        player_health = starting_health;
        attack_time_window = isAI ? 1.0f : attack_time_window;// for less aggressive AI TODO: put in difficulty of AI
        player_inputs = new PlayerInputDTO();
        playersToAttack = new Dictionary<string, RacePlayer>();

        gameEventsUI = AppConfig.findOnly<GameEventsUI>();
        placementManager = AppConfig.findOnly<PlacementManager>();
        catcher = AppConfig.findOnly<MachineCatcher>();

        // Set up variables referring to children
        electricalEffect = transform.GetComponentInChildren<ElectricalEffect>();
        shipRenderer = transform.Find("Ship").gameObject.GetComponent<MeshRenderer>();
        playerTrail = shipRenderer.transform.Find("Light").GetComponent<Light>();
        CameraPath = transform.Find("CameraPath").GetComponent<BezierSpline>();
        explosion = transform.Find("explosion").GetComponent<Explosion>();
        swipeTrail = shipRenderer.transform.Find("AttackTrail").GetComponent<SwipeTrail>();
        electricalEffect = transform.Find("ElectricEffect").GetComponent<ElectricalEffect>();
        boostSound = transform.Find("FlyByAudio").GetComponent<AudioObject>();
        bumpSound = transform.Find("RacerHitAudio").GetComponent<AudioObject>();

        // Set up ship renderer properties
        base_ship_rotation = shipRenderer.transform.localRotation;
        shipMaterial = shipRenderer.material;
        shipMaterial.SetColor("_Tint", baseTint);
        redMaterial = new Material(Shader.Find("Transparent/Diffuse"));
        redMaterial.color = new Color32(1, 0, 0, 1);

        tilt = Quaternion.identity;

        if (Physics.Raycast(transform.position, -transform.up, out downHit, rayCastDistance, AppConfig.groundMask))
        {
            transform.position = downHit.point + hover_height * transform.up;
            transform.rotation = Quaternion.FromToRotation(transform.up, downHit.normal) * transform.rotation;
            global_orientation = transform.rotation;
            previousGravity = -downHit.normal;

            // set last checkpoint on startup in case we immediately die,
            // but set the place we return to as the starting point
            lastCheckPoint = placementManager.firstCheckPoint_A;
            lastCheckPointRotation = transform.rotation;
            lastCheckPointPosition = transform.position + (AppConfig.hoverHeight) * (lastCheckPointRotation * Vector3.up);
        }
        else
        {
            Debug.LogError(name + " not above track!");
        }
    }
    
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
            if ((pauseInvariantTime - timeStartReturning) >= timeSpentReturning)
            {
                status = PlayerStatus.ONTRACK;

                if (!isEffectiveAI)
                {
                    catcher.Leave();
                }
            }
            return;
        }

        checkMiscMovement();

        bool accelerating = isEffectiveAI || player_inputs.forwardButton;
        prev_up = transform.up;
        
        // if player is above the track, recalculate position and orientation
        if (Physics.Raycast(transform.position + height_above_cast * prev_up, -prev_up, out downHit,
            inFreefall ? freeFallRayCastDistance : rayCastDistance, AppConfig.groundMask))
        {
            if (status == PlayerStatus.INAIR)
            {
                bumpSound.Play();
            }

            status = PlayerStatus.ONTRACK;
            downward_speed = 0f;

            if (accelerating)
            {
                current_speed += (current_speed >= fwd_max_speed) ?
                    (
                        ((current_speed == fwd_max_speed) && !finishedWithRace) ? 0 : -fwd_boost_decel
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
            adjustOrientation();
            checkGroundMovement();
            adjustPosition();

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

                transform.rotation = tilt * global_orientation;
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
    #endregion

    #region Private Methods
    private void setLightColor()
    {
        if (current_speed <= fwd_max_speed)
        {
            playerTrail.intensity = (baseIntensity * current_speed) / fwd_max_speed;
        }
        else
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
            if (player_health <= 2f) return;

            float boost_factor = 1f;//want to use less than normal boost power if low health
            if (player_health <= boost_cost + 1)
            {
                boost_factor = player_health / (boost_cost + 1);
            }

            current_speed += boost_factor * (fwd_boost_speed - current_speed);
            boostEffects();
            lastTimeBoostPower = pauseInvariantTime;

            if (!cheat_infinite_boost)
                slowlyDamage((int)(boost_factor * boost_cost));
        }

        if (!isAI && current_speed >= fwd_max_speed)
        {
            Camera.main.transform.localPosition = PlayerToCamera;
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
            Camera.main.transform.localPosition = PlayerToCamera;
        });
    }

    private void return_to_track()
    {
        if (!isEffectiveAI)
        {
            catcher.Enter();
        }
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
        returningToTrackRotationEnd = lastCheckPointRotation;
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
                if (current_TrackPoint != null 
                    && isEffectiveAI 
                    && !spaceBar
                    && current_TrackPoint.usesAISmoothing)
                {
                    turn_angle = Vector3.Dot(current_TrackPoint.tangent, shipRenderer.transform.forward.normalized);
                    turn_angle *= AppConfig.radToDeg;
                    if (turn_angle < 0)
                    {
                        turn_angle = Math.Max(turn_angle, -2f);
                    }
                    else
                    {
                        turn_angle = Math.Min(turn_angle, 2f);
                    }

                    shipRenderer.transform.localRotation = Quaternion.AngleAxis(turn_angle, shipRenderer.transform.forward) * base_ship_rotation;
                }
                else
                {
                    shipRenderer.transform.localRotation = Quaternion.AngleAxis((spaceBar ? ship_mesh_tilt_hard_turn : ship_mesh_tilt) * turn_angle, shipRenderer.transform.forward) * base_ship_rotation;
                }
            }
        }

        global_orientation = Quaternion.Euler(0, turn_angle, 0);
    }

    private void adjustOrientation()
    {
        Vector3 desired_up = Vector3.Lerp(prev_up, downHit.normal, Time.deltaTime * pitch_smooth);
        tilt.SetLookRotation(transform.forward - Vector3.Project(transform.forward, desired_up), desired_up);
        transform.rotation = tilt * global_orientation;
        previousGravity = -downHit.normal;
    }

    private void adjustPosition()
    {
        float distance = downHit.distance - height_above_cast;
        smooth_y = Mathf.Lerp(smooth_y, hover_height - distance, Time.deltaTime * height_smooth);
        smooth_y = Mathf.Max(distance / -3, smooth_y); //sanity check on smooth_y

        transform.localPosition += prev_up * smooth_y;
        transform.position += transform.forward * (current_speed * Time.deltaTime);
    }

    private void setInputsFromAI(out float horizontal_input, out float vertical_input, out bool spaceBar)
    {
        // for h - is turning left and + is right!
        spaceBar = false;
        vertical_input = 0f;
        int nearEdge = AIUtil.checkIfNearEdge(transform.position, transform.up, current_TrackPoint);

        // Need to turn the ship away from the edge
        if (nearEdge != 0)
        {
            spaceBar = true;
            if (nearEdge > 0)
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
        // NOTE: somehow current_TrackPoint can be set to the stupid "Initial TrackPoint" if it exists
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

        if (status == PlayerStatus.INAIR)
        {
            vertical_input = prev_v + max_delta_v;
            vertical_input = vertical_input > 1 ? 1 : vertical_input;
        }
        else if (prev_v != 0)
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
        float timeWindow = isAI ? ai_attack_allowed_cooldown : attack_time_window;
        if (pauseInvariantTime - lastTimeAttacked > timeWindow && playersToAttack.Count > 0)
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
    #endregion

    #region Events
    void OnTriggerEnter(Collider coll)
    {
        switch (coll.gameObject.tag)
        {

            //create a velocity vector from bouncing off the wall
            case "CollissionWall":
                if (status == PlayerStatus.RETURNINGTOTRACK) return;

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
                }
                else
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
                if (status == PlayerStatus.RETURNINGTOTRACK) return;

                current_speed = fwd_boost_speed;
                boostEffects();
                coll.gameObject.GetComponent<BoostPanel>().boostAnimation();
                break;

            //when we cross the finish line
            case "FinishLine":
                if (status == PlayerStatus.RETURNINGTOTRACK) return;

                StartedLap1 = true;
                placementManager.crossFinish(this);
                break;

            //when we hit a trackpoint trigger
            case "TrackPoint":
                if (status == PlayerStatus.RETURNINGTOTRACK) return;

                TrackPoint trackPoint = coll.gameObject.GetComponent<TrackPoint>();

                //AI should only stick to one path
                if (!isEffectiveAI || AIPathChoice == trackPoint.pathChoice)
                {
                    current_TrackPoint = trackPoint;
                }

                if (placementManager.updateTrackPoint(this, trackPoint))
                {
                    lastCheckPoint = trackPoint;
                    lastCheckPointRotation = Quaternion.LookRotation(lastCheckPoint.tangent, transform.up);
                    lastCheckPointPosition = transform.position + (AppConfig.hoverHeight) * transform.up;
                }
                break;

            //attack or bump other player
            case "Player":
                if (status == PlayerStatus.RETURNINGTOTRACK) return;

                if (!playersToAttack.ContainsKey(coll.name))
                {
                    playersToAttack.Add(coll.name, coll.gameObject.GetComponent<RacePlayer>());
                }
                bump(coll.gameObject.GetComponent<RacePlayer>(), false);

                //the AI may decide to attack another player >:)
                //TODO: add more special effects for this. It's not obvious they're attacking
                if (isAI)
                {
                    // only allow AI to attack within 1 second intervals every <ai_attack_allowed_inc> seconds
                    if (((int)pauseInvariantTime % ai_attack_allowed_inc) != 0)
                        return;

                    callAfterSeconds(0.1f, () =>
                    {
                        attack_player();
                    });
                }
                break;

            //Make sure falling player doesn't fall through ground
            case "Ground":
                if (status == PlayerStatus.INAIR || inFreefall)
                {
                    if (Physics.Raycast(transform.position - 5 * transform.forward, transform.forward, out downHit, 15, AppConfig.groundMask))
                    {
                        float rho = Vector3.Dot(transform.forward, downHit.normal);
                        status = PlayerStatus.ONTRACK;
                        transform.position = downHit.point + downHit.normal * hover_height;
                        current_speed *= 1f / (1 + 3 * rho * rho);
                        downward_speed = 0f;
                    }
                }
                break;
            case "DeathZone":
                if (status == PlayerStatus.INAIR)
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
                callAfterSeconds(0.4f, () =>
                {
                    if (playersToAttack.ContainsKey(coll.name))
                        playersToAttack.Remove(coll.name);
                });
                break;

            //don't do anything for most tags
            default:
                break;
        }

    }
    #endregion

    #region Public Methods
    /*
     * Needed to pass inputs to the player
     */
    public void passPlayerInputs(PlayerInputDTO _player_inputs)
    {
        player_inputs = _player_inputs;
    }

    /**
     * called when one player attacks another from _dir direction with _damage
     */
    public void attack(string attackerName, Vector3 _dir, float _damage, bool _attacking)
    {
        _damage *= damage_damper;
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
        while (_damage > 0)
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
    #endregion
}
