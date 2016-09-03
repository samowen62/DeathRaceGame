using UnityEngine;
using System.Collections;

public class InputControls : MonoBehaviour {

    private KeyCode[] KeysDown;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
            Debug.Log("space key was pressed");
    }
}
