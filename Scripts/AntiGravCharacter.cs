using UnityEngine;
using System.Collections;

public class AntiGravCharacter : MonoBehaviour {

    private float speed = 1f;
    private float jumpSpeed = 1f;
    private float gravity = 16f;

    //will have to assign somehow else
    private float halfHeight = 1f;

    private const float jumpsBetweenWindow = 0.25f;
    private const float jumpDuration = 0.1f;

    private float lastJumpTime;

    public bool isCameraMovementBlocked { get; set; }

    public bool isGrounded = true;

    private Vector3 moveDirection = Vector3.zero;

    //private CharacterController character;

    private CameraControl mainCamera;

    private Vector3 downVec;
    private Vector3 sideVec;
    private Vector3 fwdVec;

    private RaycastHit downHit;

    // Use this for initialization
    void Awake()
    {
        isCameraMovementBlocked = false;
        lastJumpTime = Time.realtimeSinceStartup;

        //character = GetComponent<CharacterController>();
        mainCamera = FindObjectOfType<CameraControl>();

        //initiailze orientation
        //TODO: generalize this to not start on flat surface
        fwdVec = Vector3.forward;
        downVec = Vector3.down;
        sideVec = Vector3.left;
    }

    // Update is called once per frame
    void Update()
    {
        

       // moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDirection = Input.GetAxis("Horizontal") * sideVec + Input.GetAxis("Vertical") * fwdVec;
        moveDirection *= speed;
        if (isGrounded)//need to determine if grounded
        {
            if (Input.GetButton("Jump") && canJump())
            {
                //TODO: x/y jump speeds
                //moveDirection.y = jumpSpeed;
                moveDirection += downVec * -jumpSpeed;
                lastJumpTime = Time.realtimeSinceStartup;
            }
        }
        else
        {
            if (stillJumping())
            {
                // moveDirection.y = jumpSpeed;
                moveDirection += downVec * -jumpSpeed;
            }else
            {
                moveDirection += downVec * gravity * Time.deltaTime;
            }
        }
        // moveDirection += downVec * -halfHeight;

        //Debug.Log(moveDirection.x);
        //Debug.Log(moveDirection.y);
        //Debug.Log(moveDirection.z);

        if (Physics.Raycast(transform.position, downVec, out downHit, 20))
        {
            downVec = -downHit.normal;

            //must keep fwdVec a unity normal. Will have to update on every frame
            sideVec = Vector3.Cross(fwdVec, downVec);
            float moveDist = halfHeight - downHit.distance;
            if (moveDist > 0 && !stillJumping())
            {
                moveDirection -= downVec * moveDist;
            }
        }

        transform.position += moveDirection;

        if (!isCameraMovementBlocked)
        {
            mainCamera.cameraUpdate();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            isGrounded = true;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            isGrounded = true;
        }
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
