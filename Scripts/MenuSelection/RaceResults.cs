using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RaceResults : MonoBehaviour {

    private GameData gameData;

    private GameObject placementDisplay;
    private RaceResultFinishUI placementDisplayContainer;
    private Image winnerPicture;

    public SceneFade scenefade;

    public AudioObject hoverSound;

    void Awake () {
        //TODO: do something for the winner picture
        gameData = FindObjectOfType<GameData>();

        if (gameData == null)
        {
            Debug.LogError("No GameData object found!!");
        }

        winnerPicture = transform.Find("WinnerPicture").gameObject.GetComponent<Image>();
        placementDisplay = transform.Find("PlacementDisplay").gameObject;
        placementDisplayContainer = placementDisplay.transform.Find("PlacementDisplayContainer")
            .gameObject.GetComponent<RaceResultFinishUI>();

        Button menuButton = GameObject.Find("BackToMenuText").GetComponent<Button>();
        menuButton.onClick.AddListener(delegate () { SyncLoadLevel("MainMenu"); });
        UIUtil.addTrigger(() => hoverSound.Play(), EventTriggerType.PointerEnter, menuButton, gameObject);


        List<string> playersOrdered = gameData.getPlayersByPlacement();
        int i = 0;
        playersOrdered.ForEach(e =>
        {
            int ending_y = 120 - i * 70;
            RaceResultFinishUI placementDisplayContainerCopy =
                Instantiate(
                    placementDisplayContainer, 
                    placementDisplayContainer.transform.position - new Vector3(0, ending_y, 0),
                    Quaternion.identity, 
                    placementDisplay.transform);
            placementDisplayContainerCopy.startAnimation(i * 0.3f, e, i + 1, ending_y);
            i++;
        });
    }

    private void SyncLoadLevel(string levelName)
    {
        StartCoroutine(Load(levelName));
    }

    //TODO: fix this!! and in TrackMenu.cs
    IEnumerator Load(string levelName)
    {
        scenefade.fade();
        yield return new WaitForSeconds(scenefade.duration);
        SceneManager.LoadSceneAsync(levelName);
    }

}
