using UnityEngine;
using System.Collections;

public class BobScript : PausableBehaviour {

    public float distance = 20f;
    public float speed = 4f;
    public float offset = 0f;

    private Vector3 initialPosition;

    protected override void _awake()
    {
        initialPosition = transform.localPosition;
    }

    protected override void _update()
    {
        transform.localPosition = initialPosition + new Vector3(0, distance * Mathf.Sin(speed * pauseInvariantTime + offset));
    }

}
