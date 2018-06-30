using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeTrail : PausableBehaviour
{
    private bool isSwiping = false;
    private float trailTime;
    private float yCoord = 0;

    private float swipeStartTime = 0f;
    private float swipeTime = 0.1f;
    private float swipeDistance = -6f; 
    private Vector3 startVec;

    private TrailRenderer trail;

    public void swipe(bool left) 
    {
        if (isSwiping) return;

        isSwiping = true;
        yCoord = 5f * (left ? 1 : -1);

        trail.time = trailTime;
        swipeStartTime = pauseInvariantTime;
        transform.localPosition = new Vector3(4, yCoord, 0);
        
        StartCoroutine(SwipeRoutine());
    }

    // Use this for initialization
    protected override void _awake () {
        trail = GetComponent<TrailRenderer>();
        trailTime = trail.time;
        trail.time = 0f;
    }

    IEnumerator SwipeRoutine()
    {

        float progress = pauseInvariantTime - swipeStartTime;
        while (progress < swipeTime)
        {
            transform.localPosition = new Vector3((progress / swipeTime) * swipeDistance, yCoord, 0f);
            progress = pauseInvariantTime - swipeStartTime;
            yield return null;
        }

        isSwiping = false;
        trail.time = 0f;
        transform.localPosition = Vector3.zero;
    }

}
