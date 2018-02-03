using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTrigger : PausableBehaviour
{
    [Tooltip("Smallest amount of time allowed before this can be triggered again")]
    public float minTimeBetweenTriggers = 1f;
    private float lastTimeTriggered = 0f;

    [Tooltip("Time this movement lasts")]
    public float movementTime = 1f;

    public BezierSpline objectPath;

    public GameObject movingObject;

    private Vector3 upDirection;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private void Awake()
    {
        upDirection = transform.up;
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (pauseInvariantTime - lastTimeTriggered > minTimeBetweenTriggers)
        {
            lastTimeTriggered = pauseInvariantTime;
            StartCoroutine(MoveAlongPath());
        }
    }

    IEnumerator MoveAlongPath()
    {
        OnTriggerStart();

        while (pauseInvariantTime - lastTimeTriggered <= movementTime)
        {
            float t = (pauseInvariantTime - lastTimeTriggered) / movementTime;
            movingObject.gameObject.transform.position = objectPath.GetPoint(t);

            Vector3 pathNormal = objectPath.GetDirection(t);
            movingObject.gameObject.transform.rotation = Quaternion.LookRotation(pathNormal, upDirection);
            yield return null;
        }

        OnTriggerEnd();

        transform.position = initialPosition;
        transform.rotation = initialRotation;
        yield return null;
    }

    protected virtual void OnTriggerStart()
    {

    }

    protected virtual void OnTriggerEnd()
    {

    }
}
