using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdFlock : PathTrigger
{
    public Bird[] birds;

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
