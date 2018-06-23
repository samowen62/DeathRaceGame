using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour {

    public AudioObject selectSound;

    public AudioObject hoverSound;

    public SceneFade scenefade;

    private Button startGameButton;
    private Button recordsGameButton;
    private Button exitGameButton;

    private const string START_GAME_BUTTON = "StartGame";
    private const string RECORDS_NAME = "Records";
    private const string END_GAME_BUTTON = "EndGame";

    private bool startBlocked = false;

    // Use this for initialization
    void Awake () {
        Debug.Log("Death Race Game Start!!");
        Button [] buttons = FindObjectsOfType<Button>();

        foreach(var button in buttons)
        {
            Debug.Log(button.name);
            switch (button.name)
            {
                case START_GAME_BUTTON:
                    startGameButton = button;
                    startGameButton.onClick.AddListener(delegate () { startButtonClicked(); });
                    UIUtil.addTrigger(() => hoverSound.Play(), EventTriggerType.PointerEnter, startGameButton, gameObject);
                    break;
                case RECORDS_NAME:
                    recordsGameButton = button;
                    recordsGameButton.onClick.AddListener(delegate () { recordsButtonClicked(); });
                    UIUtil.addTrigger(() => hoverSound.Play(), EventTriggerType.PointerEnter, recordsGameButton, gameObject);
                    break;
                case END_GAME_BUTTON:
                    exitGameButton = button;
                    exitGameButton.onClick.AddListener(delegate () { endGameButtonClicked(); });
                    UIUtil.addTrigger(() => hoverSound.Play(), EventTriggerType.PointerEnter, exitGameButton, gameObject);
                    break;
                default:
                    Debug.LogWarning("unused button component: " + button.name);
                    break;
            }
        }
    }

    private void startButtonClicked()
    {
        if (!startBlocked)
        {
            selectSound.Play();
            startBlocked = true;
            Debug.Log("Started Test Track sequence");

            SyncLoadLevel(AppConfig.MENU_TRACK);
        }
    }

    private void recordsButtonClicked()
    {
        selectSound.Play();
        startBlocked = true;
        Debug.Log("Records Clicked");

        SyncLoadLevel(AppConfig.MENU_RECORDS);
    }

    public void endGameButtonClicked()
    {
        Debug.Log("Exiting Game");
        Application.Quit();
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
