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


        if (Physics.Raycast(transform.position, -rigidBody.transform.up, out downHit, rayCastDistance))
        {

            moveDirection = speed * rigidBody.transform.forward;

            //maybe try smoothing the normals by saving the last few? It's an unwritten rule that there won't be any sharp surfaces
            float downForce = (downHit.distance - hoverHeight);
            downForce = Mathf.Max(maxAntiGravMagnitude, downForce) * gravity;
            previousGravity = downHit.normal * downForce;

            rigidBody.AddForce(previousGravity);
            rigidBody.AddForce(moveDirection - (velocityCompensation * rigidBody.velocity));

            //hard turns should increase turnSpeed
            if (Input.GetAxis("Horizontal") != 0)
            {
                if (Input.GetAxis("Horizontal") > 0)
                {
                    rigidBody.AddTorque(turnSpeed * transform.up);
                }
                else
                {
                    rigidBody.AddTorque(-turnSpeed * transform.up);
                }
            }
            else
            {
                rigidBody.angularVelocity = Vector3.zero;
            }

            rigidBody.AddTorque(torqueSpeed * Vector3.Cross(rigidBody.transform.forward, downHit.normal));

        }
        else
        {
            //need to make sure this is the ground though
            //Debug.Log("fell");
            rigidBody.AddForce(previousGravity * 10f);
        }

    }

}
