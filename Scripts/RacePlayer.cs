using UnityEngine;
using System.Collections;


public class RacePlayer : MonoBehaviour {

    public float speed = 15f;
    public float turnSpeed = 15f;
    public float torqueSpeed = 0.1f;
    public float gravity = -10f;
    public float hoverHeight = 0.5f;
    public float velocityCompensation = 0.5f;
    public float maxAntiGravMagnitude = -0.1f;
    public float rayCastDistance = 20f;
    public float hardDriftSpeed = 10f;

    private Rigidbody rigidBody;

    private Vector3 previousGravity;
    private Vector3 torqueVector;
    private Vector3 moveDirection = Vector3.zero;

    private RaycastHit downHit;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();

        if (Physics.Raycast(transform.position, -rigidBody.transform.up, out downHit, rayCastDistance))
        {
            previousGravity = -downHit.normal;
        }
    }


    void Update()
    {

        /**
         * An alernative approach might be to find the desired next position based on the track
         * and apply a force/torque that would complement this.
         * 
         * If no position is found on the track we can just have the default gravity and no angular momentum
         * 
         */
        if (Physics.Raycast(transform.position, -rigidBody.transform.up, out downHit, rayCastDistance))
        {

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
            Debug.Log(Vector3.Dot(right, downHit.normal));//not 0???!
            rigidBody.MoveRotation(Quaternion.LookRotation(right, downHit.normal));
            

        }
        else
        {
            //need to make sure this is the ground though
            //Debug.Log("fell");
            rigidBody.AddForce(previousGravity);
        }
    }

}
