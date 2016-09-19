using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

    public AntiGravCharacter target;

    private Vector3 fromTarget;

	// Use this for initialization
	void Start () {
        fromTarget = transform.position - target.transform.position;
	}

    public void cameraUpdate()
    {
        if (target)
        {
            transform.position = target.transform.position + target.getRelativeCameraPos();
            //transform.position = Vector3.Slerp(transform.position, target.transform.position + target.getRelativeCameraPos(), Time.deltaTime * 0.5f);

            //TODO: don't set it explicitly but slowly move in the direction
            //transform.rotation = Quaternion.Slerp(transform.rotation, target.transform.rotation, Time.deltaTime * 2.5f);
            transform.rotation = target.transform.rotation;
        }
    }
}
