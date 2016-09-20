using UnityEngine;
using System.Collections;

//TODO: 
public class AntiGravCharacter : MonoBehaviour {

    private float speed = 1f;
    private float jumpSpeed = 1f;
    private float gravity = 16f;

    private float turnSpeed = 5f;

    //TODO: will have to assign somehow else
    private float halfHeight = 2f;

    private float rotationLeeway = 1f;

    private const float jumpsBetweenWindow = 0.25f;
    private const float jumpDuration = 0.1f;

    private float lastJumpTime;

    public bool isGrounded = true;

    private Vector3 moveDirection = Vector3.zero;
    private Vector3 nextPosition;

    private CameraControl mainCamera;

    private Vector3 upVec;
    private Vector3 sideVec;
    private Vector3 fwdVec;

    private RaycastHit downHit;

    // Use this for initialization
    void Awake()
    {
        lastJumpTime = Time.realtimeSinceStartup;

        mainCamera = FindObjectOfType<CameraControl>();

        //initiailze orientation
        fwdVec  = gameObject.transform.forward;
        upVec   = gameObject.transform.up;
        sideVec = Vector3.Cross(upVec, fwdVec);
    }


    void FixedUpdate()
    {
        moveDirection =  Input.GetAxis("Vertical") * fwdVec;
        moveDirection *= speed;
        if (isGrounded)//need to determine if grounded
        {
            if (Input.GetButton("Jump") && canJump())
            {
                //TODO: x/y jump speeds
                isGrounded = false;
                moveDirection += upVec * jumpSpeed;
                lastJumpTime = Time.realtimeSinceStartup;
            }
        }
        else
        {
            //Debug.Log("in the air");
            if (stillJumping())
            {
                moveDirection += upVec * jumpSpeed;
            }else
            {
                //Debug.Log("falling");
                moveDirection -= upVec * gravity * Time.deltaTime;
            }
        }

        fwdVec  = Quaternion.AngleAxis(Input.GetAxis("Horizontal") * turnSpeed, upVec) * fwdVec;
        sideVec = Quaternion.AngleAxis(Input.GetAxis("Horizontal") * turnSpeed, upVec) * sideVec;

        nextPosition = transform.position + moveDirection;

        if (Physics.Raycast(nextPosition, -upVec, out downHit, 20))
        {
            upVec = downHit.normal;
            Debug.Log(upVec.magnitude);

            //project fwdVec on tangent plane
            sideVec = Vector3.Cross(upVec, fwdVec);
            sideVec.Normalize();

            fwdVec = Vector3.Cross(sideVec, upVec);
            fwdVec.Normalize();

            //will have to have a tolerance for donwHit.distance above the ground
            if (!stillJumping())
            {
                //Debug.Log("adjusting");
                isGrounded = true;
                nextPosition = downHit.point + upVec * halfHeight;
            }
        }else
        {
            //need to make sure this is the ground though
            Debug.Log("fell");
            isGrounded = false;
        }

        transform.position = nextPosition;
        transform.rotation = Quaternion.LookRotation(fwdVec, upVec);
        
    }

    private bool canJump()
    {
        return Time.realtimeSinceStartup - lastJumpTime > jumpsBetweenWindow;
    }

    private bool stillJumping()
    {
        return Time.realtimeSinceStartup - lastJumpTime < jumpDuration;
    }
}
