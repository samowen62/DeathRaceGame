using UnityEngine;
using System.Collections;

public abstract class PausableBehaviour : MonoBehaviour {

    protected abstract void _awake();

    protected abstract void _update();

    void Awake () {
        _awake();
    }
	
	void FixedUpdate () {
        _update();
    }
}
