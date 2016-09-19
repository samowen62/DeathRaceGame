using UnityEngine;
using System.Collections;

//TODO: 
public class AntiGravCharacter : MonoBehaviour {

    private float speed = 1f;
    private float jumpSpeed = 1f;
    private float gravity = 16f;

    private float turnSpeed = 5f;

    private const float cameraZ = -10f;
    private const float cameraY = 3f;

    //TODO: will have to assign somehow else
    private float halfHeight = 1f;

    private const float jumpsBetweenWindow = 0.25f;
    private const float jumpDuration = 0.1f;

    private float lastJumpTime;

    public bool isCameraMovementBlocked { get; set; }

    public bool isGrounded = true;

    private Vector3 moveDirection = Vector3.zero;

    private CameraControl mainCamera;

    private Vector3 upVec;
    private Vector3 sideVec;
    private Vector3 fwdVec;

    private RaycastHit downHit;

    // Use this for initialization
    void Awake()
    {
        isCameraMovementBlocked = false;
        lastJumpTime = Time.realtimeSinceStartup;

        mainCamera = FindObjectOfType<CameraControl>();

        //initiailze orientation
        //TODO: generalize this to not start on flat surface
        fwdVec = Vector3.forward;
        upVec = Vector3.up;
        sideVec = Vector3.left;
    }

    // Update is called once per frame
    void Update()
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

        //Debug.Log(moveDirection.x);
        //Debug.Log(moveDirection.y);
        //Debug.Log(moveDirection.z);

        if (Physics.Raycast(transform.position, -upVec, out downHit, 20))
        {
            upVec = downHit.normal;
            //project fwdVec on tangent plane

            //must keep fwdVec a unity normal. Will have to update on every frame
            sideVec = Vector3.Cross(upVec, fwdVec);
            sideVec.Normalize();

            fwdVec = Vector3.Cross(sideVec, upVec);
            fwdVec.Normalize();

            float moveDist = halfHeight - downHit.distance;
            if (moveDist > 0 && !stillJumping())
            {
                //Debug.Log("adjusting");
                isGrounded = true;
                moveDirection += upVec * moveDist;
            }
        }else
        {
            //need to make sure this is the ground though
            //Debug.Log("fell");
            isGrounded = false;
        }

        transform.position += moveDirection;

        Quaternion rotation = Quaternion.LookRotation(fwdVec, upVec);

        float dd = 10f;
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * dd);
        Debug.Log(dd);
        //transform.rotation = rotation;

        if (!isCameraMovementBlocked)
        {
            mainCamera.cameraUpdate();
        }
    }

    public Vector3 getRelativeCameraPos()
    {
        return cameraZ * fwdVec + cameraY * upVec;
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
