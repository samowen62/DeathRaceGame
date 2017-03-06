using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public abstract class PausableBehaviour : MonoBehaviour {

    private float timePaused;
    private float totalTimePaused = 0f;

    protected float pauseInvariantTime {
        get
        {
            if (_behaviorBlocked)
            {
                return timePaused;
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

    /* 
     * This is a dictionary of timestamps an inheriting class may want to increment when the game
     * is paused in order to make sure they are the same amount in the past 
     */
    protected Dictionary<string, float> pauseInvariantTimestamps;

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
                //if before awake() is called
                if (pauseInvariantTimestamps == null)
                {
                    return;
                }

                totalTimePaused += Time.fixedTime - timePaused;

                //Must gather keys first to avoid out of sync exception
                List<string> keys = new List<string>();
                foreach (var entry in pauseInvariantTimestamps)
                {
                    keys.Add(entry.Key);
                }

                foreach (var key in keys)
                {
                    pauseInvariantTimestamps[key] += Time.fixedTime - timePaused;
                }

                onUnPause();
            }
            _behaviorBlocked = value;
        }
    }

    void Awake () {
        pauseInvariantTimestamps = new Dictionary<string, float>();

        _awake();
    }
	
	void FixedUpdate () {

        if (_behaviorBlocked)
        {
            return;
        }

        _update();
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
