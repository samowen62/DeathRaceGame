using UnityEngine;
using System;
using System.Collections;

public abstract class PausableBehaviour : MonoBehaviour {

    private float timePaused;
    private float totalTimePaused = 0f;

    protected float pauseInvariantTime {
        get
        {
            if (_behaviorBlocked)
            {
                return timePaused - totalTimePaused;
            }
            else
            {
                return Time.fixedTime - totalTimePaused;
            }
        }
        set
        {
        }
    }


    /* This is for pausing the game */
    protected bool _behaviorBlocked;
    public bool behaviorBlocked
    {
        get
        {
            return _behaviorBlocked;
        }
        set
        {
            if (value)
            {
                onPause();
                timePaused = Time.fixedTime;
            }
            else
            {
                totalTimePaused += Time.fixedTime - timePaused;
                onUnPause();
            }
            _behaviorBlocked = value;
        }
    }

    void Awake () {
        _awake();
    }
	
	void FixedUpdate () {

        if (_behaviorBlocked)
        {
            return;
        }

        _update();
    }

    /**
     * Utility function used to call a function after a specified amount 
     * of time. The time is made to be pause invariant additionally
     */
    protected void callAfterSeconds(float seconds, Action func)
    {
        StartCoroutine(callFunc(seconds, func));
    }

    private IEnumerator callFunc(float seconds, Action func)
    {
        float timeToStop = pauseInvariantTime + seconds;
        yield return new WaitUntil(() => timeToStop <= pauseInvariantTime);
        func();
    }



    protected virtual void _awake()
    {
    }

    protected virtual void _update()
    {
    }

    protected virtual void onPause()
    {
    }

    protected virtual void onUnPause()
    {
    }
}
