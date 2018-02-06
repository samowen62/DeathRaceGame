using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdFlock : PathTrigger
{
    private Bird[] birds;

    protected override void OnAwake()
    {
        birds = GetComponentsInChildren<Bird>();
    }

    protected override void OnTriggerStart()
    {
        foreach(var bird in birds)
        {
            bird.StartPlaying();
        }
    }

    protected override void OnTriggerEnd()
    {
        foreach (var bird in birds)
        {
            bird.StopPlaying();
        }
    }
}
