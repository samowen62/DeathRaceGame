using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public abstract class PausableBehaviour : MonoBehaviour {

    private float timePaused;

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
                timePaused = Time.fixedTime;
            }
            else
            {
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
            }

            _behaviorBlocked = value;
        }
    }

    protected abstract void _awake();

    protected abstract void _update();

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
}
