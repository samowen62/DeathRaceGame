using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour {

    public AudioObject selectSound;

    public AudioObject hoverSound;

    public SceneFade sceneFade;

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

            LoadLevel(AppConfig.MENU_TRACK);
        }
    }

    private void recordsButtonClicked()
    {
        selectSound.Play();
        startBlocked = true;
        Debug.Log("Records Clicked");

        LoadLevel(AppConfig.MENU_RECORDS);
    }

    public void endGameButtonClicked()
    {
        Debug.Log("Exiting Game");
        Application.Quit();
    }

    private void LoadLevel(string levelName)
    {
        StartCoroutine(PerformLoad(levelName));
    }

    IEnumerator PerformLoad(string levelName)
    {
        sceneFade.fade();
        yield return new WaitForSeconds(sceneFade.duration);
        yield return AppConfig.Load(levelName);
    }
}
