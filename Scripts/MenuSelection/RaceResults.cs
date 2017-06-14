using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RaceResults : MonoBehaviour {

    private GameData gameData;

    private GameObject placementDisplay;
   // private 
    private RaceResultFinishUI placementDisplayContainer;
    private Image winnerPicture;


    void Awake () {
        //TODO: do something for the winner picture
        gameData = FindObjectOfType<GameData>();

        if (gameData == null)
        {
            Debug.LogError("No GameData object found!!");
        }

        winnerPicture = transform.Find("WinnerPicture").gameObject.GetComponent<Image>();
        placementDisplay = transform.Find("PlacementDisplay").gameObject;
        placementDisplayContainer = placementDisplay.transform.Find("PlacementDisplayContainer").gameObject.GetComponent<RaceResultFinishUI>();

        List<string> playersOrdered = gameData.getPlayersByPlacement();
        int i = 0;
        playersOrdered.ForEach(e =>
        {
            int ending_y = 120 - i * 70;
            RaceResultFinishUI placementDisplayContainerCopy = Object
                .Instantiate(placementDisplayContainer, placementDisplayContainer.transform.position - new Vector3(0, ending_y, 0)
                , Quaternion.identity, placementDisplay.transform);
            placementDisplayContainerCopy.startAnimation(i * 0.3f, e, i + 1, ending_y);
            i++;
        });
    }
	
}
