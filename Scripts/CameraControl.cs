using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

    public GameObject target;

    private Vector3 fromTarget;

	// Use this for initialization
	void Start () {
        fromTarget = transform.position - target.transform.position;
	}

    public void cameraUpdate()
    {
        if (target)
        {
            transform.position = target.transform.position + fromTarget;
        }
    }
}
