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
    public float speed = 15f;
    public float turnSpeed = 15f;
    public float torqueSpeed = 0.1f;
    public float gravity = -10f;
    public float hoverHeight = 0.5f;
    public float velocityCompensation = 0.5f;
    public float maxAntiGravMagnitude = -0.1f;
    public float rayCastDistance = 20f;
    public float hardDriftSpeed = 10f;
    public float returningToTrackSpeed = 0.001f;
    public float timeAllowedNotOnTrack = 2.5f;
    public float timeSpentReturning = 3f;

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
                if (lastTimeOnGround != -1)
                {
                    lastTimeOnGround += (Time.fixedTime - timePaused);
                }             
                timeStartReturning += (Time.fixedTime - timePaused);
            }
            _behaviorBlocked = value;
        }
    }

    private Rigidbody rigidBody;

    private Vector3 previousGravity;
    private Vector3 torqueVector;
    private Vector3 moveDirection = Vector3.zero;

    private RaycastHit downHit;

    private CheckPoint lastCheckPoint;

    private int bitMask = 1 << 8;

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
        rigidBody = GetComponent<Rigidbody>();  

        if (Physics.Raycast(transform.position, -rigidBody.transform.up, out downHit, rayCastDistance, bitMask))
        {
            previousGravity = -downHit.normal;
        }
    }


    void FixedUpdate()
    {
        if (behaviorBlocked)
        {
            rigidBody.Sleep();
            return;
        }

    
        if (returningToTrack)
        {
            float deltaTime = (Time.fixedTime - timeStartReturning) * returningToTrackSpeed;
            rigidBody.MovePosition(Vector3.Lerp(returningToTrackPositionBegin, returningToTrackPositionEnd, deltaTime));
            rigidBody.MoveRotation(Quaternion.Lerp(returningToTrackRotationBegin, returningToTrackRotationEnd, deltaTime));
            returningToTrack = (Time.fixedTime - timeStartReturning) < timeSpentReturning;
            return;
        }

        /**
         * An alernative approach might be to find the desired next position based on the track
         * and apply a force/torque that would complement this.
         * 
         * If no position is found on the track we can just have the default gravity and no angular momentum
         * 
         */
        if (Physics.Raycast(transform.position, -rigidBody.transform.up, out downHit, rayCastDistance, bitMask))
        {
            //reset since are on ground
            returningToTrack = false;
            falling = false;
            lastTimeOnGround = -1f;

            moveDirection = speed * rigidBody.transform.forward;

            //maybe try smoothing the normals by saving the last few? It's an unwritten rule that there won't be any sharp surfaces
            float downForce = (downHit.distance - hoverHeight);
            downForce = Mathf.Max(maxAntiGravMagnitude, downForce) * gravity;
            previousGravity = downHit.normal * gravity;

            rigidBody.AddForce(downHit.normal * downForce);
            rigidBody.AddForce(moveDirection - (velocityCompensation * rigidBody.velocity));

            //hard drifts (hard turns for now)
            if (Input.GetAxis("Hard Drift") > 0)
            {
                rigidBody.AddForce(hardDriftSpeed * rigidBody.transform.right);
            }
            else if (Input.GetAxis("Hard Drift") < 0)
            {
                rigidBody.AddForce(-hardDriftSpeed * rigidBody.transform.right);
            }

            Vector3 right = rigidBody.transform.forward;
            right = (right - Vector3.Dot(right, downHit.normal) * downHit.normal).normalized;
            //need to project right onto tangent plane!!!

            //hard turns should increase turnSpeed
            if (Input.GetAxis("Horizontal") != 0)
            {
                
                if (Input.GetAxis("Horizontal") > 0)
                {
                    right = Quaternion.AngleAxis(turnSpeed, downHit.normal) * right;
                    //upwards = upwards - (Vector3.Dot(right, downHit.normal) * right);
                   // upwards.Normalize();
                    //rigidBody.AddTorque(turnSpeed * transform.up);
                }
                else
                {
                    right = Quaternion.AngleAxis(-turnSpeed, downHit.normal) * right;
                    //upwards = upwards - (Vector3.Dot(right, downHit.normal) * right);
                    //upwards.Normalize();
                    //rigidBody.AddTorque(-turnSpeed * transform.up);
                }
            }
            else
            {
                //rigidBody.angularVelocity = Vector3.zero;
            }

            //this isn't working too well :(
            //rigidBody.AddTorque(torqueSpeed * Vector3.Cross(rigidBody.transform.forward, downHit.normal));
            //Debug.Log(Vector3.Dot(right, downHit.normal));//not 0???!
            rigidBody.MoveRotation(Quaternion.LookRotation(right, downHit.normal));
            

        }
        else
        {
            //only set this on the first frame that there is a miss
            if (lastTimeOnGround == -1f)
            {
                Debug.Log("Player left contact with track");
                falling = true;
                lastTimeOnGround = Time.fixedTime;
            } 
            //if we need to go back to the track no longer do a raycast  
            else if ((Time.fixedTime - lastTimeOnGround) > timeAllowedNotOnTrack)
            {
                Debug.Log("returning");
                //set to -1 as a flag
                lastTimeOnGround = -1f;
                timeStartReturning = Time.fixedTime;
                returningToTrack = true;
                falling = false;
                returningToTrackRotationBegin = rigidBody.transform.rotation;
                returningToTrackPositionBegin = rigidBody.transform.position;
                returningToTrackRotationEnd = Quaternion.LookRotation(lastCheckPoint.trackForward, lastCheckPoint.trackNormal);
                returningToTrackPositionEnd = lastCheckPoint.trackPoint + lastCheckPoint.trackNormal;
                rigidBody.velocity = Vector3.zero;
                rigidBody.angularVelocity = Vector3.zero;
            }
            else {
                rigidBody.AddForce(previousGravity);
            }
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        switch (coll.gameObject.tag)
        {
            case "CheckPoint":
                lastCheckPoint = coll.gameObject.GetComponent<CheckPoint>();
                Debug.Log("Reached checkpoint " + lastCheckPoint.name);
                break;
        }       
    }

}
