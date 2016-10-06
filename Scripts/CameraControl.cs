using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

    public MonoBehaviour target;

    private float cameraLeeway = 1f;

    private Vector3 cameraPosition;
    private Vector3 relativePosition;

    private Rigidbody rigidbody;

    void Awake()
    {
        relativePosition = new Vector3(0, 3, -10);
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {

        //rigidbody.MovePosition(target.getRelativeCameraPosition());
        rigidbody.MoveRotation(target.transform.rotation);
    }

    public void cameraUpdate()
    {
        if (target)
        {
            transform.rotation = target.transform.rotation;
            transform.LookAt(target.transform);
        }
    }
}
