using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    private Button startGameButton;
    private Button exitGameButton;

    private const string START_GAME_NAME = "StartGame";
    private const string END_GAME_BUTTON = "EndGame";

    private bool startBlocked = false;
    private AsyncOperation async = null;

    // Use this for initialization
    void Start () {
        Debug.Log("Death Race Game Start!!");
        Button [] buttons = FindObjectsOfType<Button>();

        foreach(var button in buttons)
        {
            switch (button.name)
            {
                case START_GAME_NAME:
                    startGameButton = button;
                    startGameButton.onClick.AddListener(delegate () { this.startButtonClicked(); });
                    break;
                case END_GAME_BUTTON:
                    exitGameButton = button;
                    exitGameButton.onClick.AddListener(delegate () { this.endGameButtonClicked(); });
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

            SyncLoadLevel("paperEngine");
        }
    }

    public void endGameButtonClicked()
    {
        Debug.Log("Exiting Game");
        Application.Quit();
    }

    private void SyncLoadLevel(string levelName)
    {
        async = SceneManager.LoadSceneAsync(levelName);
        Load();
    }

    IEnumerator Load()
    {
        Debug.Log("progress: " + async.progress);
        yield return async;
    }
}
