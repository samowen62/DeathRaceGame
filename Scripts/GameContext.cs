using UnityEngine;
using System.Collections;

public class GameContext : MonoBehaviour {

    public StartingSequence staringSequence;

    public RacePlayer player;

    void Awake () {
        player.behaviorBlocked = true;

    }
	
	// TODO: Handle inputs through this class only
	void Update () {
        if (staringSequence.finished && player.behaviorBlocked)// and not paused. Only change if player blocked (only should call once)
        {
            player.behaviorBlocked = false;
        }
	}
}
