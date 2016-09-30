using UnityEngine;
using System.Collections;


public class RacePlayer : MonoBehaviour {

    private float speed = 20f;
    private float turnSpeed = 15f;
    private float torqueSpeed = 2.0f;
    private float gravity = -10f;

    private Rigidbody rigidBody;

    private Vector3 previousGravity;
    private Vector3 torqueVector;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 previousDirection = Vector3.zero;

    private RaycastHit downHit;

    // Use this for initialization
    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();

        if (Physics.Raycast(transform.position, -rigidBody.transform.up, out downHit, 20))
        {
            previousGravity = -downHit.normal;
        }
    }


    void Update()
    {

        if (Physics.Raycast(transform.position, -rigidBody.transform.up, out downHit, 20))
        {
            previousGravity = downHit.normal * gravity;

            //moveDirection = speed * Input.GetAxis("Vertical") * rigidBody.transform.forward;
            moveDirection = speed * rigidBody.transform.forward;

            /*
             * It's just a RigidBody with some colliders attached (not even wheels),
             *  being pushed forwards (and downwards) by a couple of forces: Thrust and down force based on speed.﻿
             *  
             *  
             *  capsule is rolling because of surface.
             *  -Changed to box collider
             */


            //Debug.Log("p: " + previousGravity);
            //Debug.Log("m : " + moveDirection);
            rigidBody.AddForce(previousGravity);
            rigidBody.AddForce(moveDirection - (0.5f * rigidBody.velocity));
            previousDirection = moveDirection;
            //rigidBody.velocity = moveDirection;

            //rigidBody.MoveRotation(rigidBody.rotation * Quaternion.AngleAxis(Input.GetAxis("Horizontal") * turnSpeed, downHit.normal));// rigidBody.transform.up));

            if (Input.GetAxis("Horizontal") != 0)
            {
                if(Input.GetAxis("Horizontal") > 0)
                {
                    rigidBody.AddTorque(turnSpeed * downHit.normal);
                }else
                {
                    rigidBody.AddTorque(-turnSpeed * downHit.normal);
                }             
            }
            else
            {
                rigidBody.angularVelocity = Vector3.zero;
            }

            /*
            float stability = 0.3f;
            Vector3 predictedUp = Quaternion.AngleAxis(
                rigidBody.angularVelocity.magnitude * Mathf.Rad2Deg * stability / torqueSpeed,
                rigidBody.angularVelocity
            ) * transform.up;
            torqueVector = Vector3.Cross(predictedUp, transform.up);
            */
            torqueVector = Vector3.Cross(downHit.normal, transform.up);
            rigidBody.AddTorque(torqueVector * torqueSpeed * torqueSpeed);

        }
        else
        {
            //need to make sure this is the ground though
            Debug.Log("fell");
            rigidBody.AddForce(previousGravity * 10f);
        }

    }
}
