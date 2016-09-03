using UnityEngine;
using System.Collections;

/**
 * Class to control the movements/actions of the Player
 */
public class Player : MonoBehaviour {

    private float speed = 0.3f;
    private float jumpSpeed = 35f;

    private const float gravity = 330f;
    private const float jumpsBetweenWindow = 0.25f;
    private const float jumpDuration = 0.1f;

    private float lastJumpTime;

    private bool isGrounded;
    public bool isCameraMovementBlocked { get; set; }

    private Vector3 playerToCamera;
    private Vector3 moveDirection = Vector3.zero;

    private CameraControl mainCamera;

    private CharacterController controller;

    // Use this for initialization
    void Awake () {
        isCameraMovementBlocked = false;
        isGrounded = true;
        lastJumpTime = Time.realtimeSinceStartup;

        mainCamera = FindObjectOfType<CameraControl>();
    }

    // Update is called once per frame
    void Update () {
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDirection *= speed;
        if (isGrounded)
        {
            if (Input.GetButton("Jump") && canJump())
            {
                moveDirection.y = jumpSpeed * Time.deltaTime;
                isGrounded = false;
                lastJumpTime = Time.realtimeSinceStartup;
            }
        }else
        {
            if (stillJumping())
            {
                Debug.Log("going up");
                moveDirection.y = jumpSpeed * Time.deltaTime;
            }
            moveDirection.y -= gravity * Time.deltaTime * Time.deltaTime;
        }
        
        moveTo(moveDirection * Time.deltaTime);
    }

    private void moveTo(Vector3 position)
    {
        gameObject.transform.position = transform.TransformPoint(moveDirection);
        GetComponent<Collider>().transform.position = transform.TransformPoint(moveDirection);
        if (!isCameraMovementBlocked)
        {
            mainCamera.cameraUpdate();
        }
    }

    void OnTriggerEnter(Collider enteredCollider)
    {
        //Landed on Ground
        if (enteredCollider.CompareTag("Ground"))
        {
            isGrounded = true;
            lastJumpTime = Time.realtimeSinceStartup;
        }
    }

    void OnTriggerExit(Collider enteredCollider)
    {
        if (enteredCollider.CompareTag("Ground"))
        {
            //TODO: check if in contact with other ground first. This will have the effect of falling
            //isGrounded = false;
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
