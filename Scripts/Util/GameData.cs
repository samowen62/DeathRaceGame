using UnityEngine;
using System.Collections.Generic;

public class GameData : MonoBehaviour {

    private const int numTracks = 2;

    private static string[] PlayerNames = new string[]
    {
        "Player 1",
        "AI 1"
    };


    //map of player names to Data objects
    public Dictionary<string, DataDTO> playerData;

	// Use this for initialization
	void Awake () {
        playerData = new Dictionary<string, DataDTO>();
        foreach(string player in PlayerNames)
        {
            playerData.Add(player, new DataDTO(numTracks));
        }
    }
}
