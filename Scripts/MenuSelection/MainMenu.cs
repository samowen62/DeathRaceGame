using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    private Button startGameButton;
    private Button optionsGameButton;
    private Button exitGameButton;

    private const string START_GAME_BUTTON = "StartGame";
    private const string OPTIONS_NAME = "Options";
    private const string END_GAME_BUTTON = "EndGame";

    private bool startBlocked = false;
    private AsyncOperation async = null;

    // Use this for initialization
    void Start () {
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
                    break;
                case OPTIONS_NAME:
                    optionsGameButton = button;
                    optionsGameButton.onClick.AddListener(delegate () { optionsButtonClicked(); });
                    break;
                case END_GAME_BUTTON:
                    exitGameButton = button;
                    exitGameButton.onClick.AddListener(delegate () { endGameButtonClicked(); });
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
            startBlocked = true;
            Debug.Log("Started Test Track sequence");

            SyncLoadLevel(AppConfig.MENU_TRACK);
        }
    }

    private void optionsButtonClicked()
    {
        Debug.Log("Options clicked");
    }

    public void endGameButtonClicked()
    {
        Debug.Log("Exiting Game");
        Application.Quit();
    }

    private void SyncLoadLevel(string levelName)
    {
        async = SceneManager.LoadSceneAsync(levelName);
        StartCoroutine(Load());
    }

    //TODO: fix this!! and in TrackMenu.cs
    IEnumerator Load()
    {
        Debug.Log("progress: " + async.progress);
        yield return async;
    }
}
