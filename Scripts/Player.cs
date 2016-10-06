using UnityEngine;
using System.Collections;

/**
 * Class to control the movements/actions of the Player
 */
public class Player : MonoBehaviour {

    private float speed = 1f;
    private float jumpSpeed = 1f;
    private float gravity = 16f;

    private const float jumpsBetweenWindow = 0.25f;
    private const float jumpDuration = 0.1f;

    private float lastJumpTime;

    public bool isCameraMovementBlocked { get; set; }

    private Vector3 moveDirection = Vector3.zero;

    private CharacterController character;

    private CameraControl mainCamera;

    // Use this for initialization
    void Awake () {
        isCameraMovementBlocked = false;
        lastJumpTime = Time.realtimeSinceStartup;

        character = GetComponent<CharacterController>();
        mainCamera = FindObjectOfType<CameraControl>();
    }

    // Update is called once per frame
    void Update () {
        Debug.Log("fsdafasfasf");
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDirection *= speed;
        if (character.isGrounded)
        {
            if (Input.GetButton("Jump") && canJump())
            {
                //TODO: x/y jump speeds
                moveDirection.y = jumpSpeed;
                lastJumpTime = Time.realtimeSinceStartup;
            }
        }
        else
        {
            if (stillJumping())
            {
                moveDirection.y = jumpSpeed;
            }
        }

        moveDirection.y -= gravity * Time.deltaTime;
        character.Move(moveDirection);

        if (!isCameraMovementBlocked)
        {
            mainCamera.cameraUpdate();
        }
    }

    void OnCollisionStay(Collision collision)
    {
        Debug.Log(collision.contacts.Length);
        ContactPoint contact = collision.contacts[0];
        Debug.Log(contact.point.x + " " + contact.point.y + " " + contact.point.z);
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
