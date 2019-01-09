using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour {

    public AudioObject selectSound;

    public AudioObject hoverSound;

    public SceneFade sceneFade;

    private const string START_GAME_BUTTON = "StartGame";
    private const string HOW_TO_PLAY = "HowToPlay";
    private const string RECORDS_NAME = "Records";
    private const string END_GAME_BUTTON = "EndGame";

    private bool startBlocked = false;

    // Use this for initialization
    void Awake () {
        Debug.Log("Game Start");
        Button [] buttons = FindObjectsOfType<Button>();

        foreach(var button in buttons)
        {
            switch (button.name)
            {
                case START_GAME_BUTTON:
                    button.onClick.AddListener(delegate () { startButtonClicked(); });
                    UIUtil.addTrigger(() => hoverSound.Play(), EventTriggerType.PointerEnter, button, gameObject);
                    break;
                case HOW_TO_PLAY:
                    button.onClick.AddListener(delegate () { howToPlayButtonClicked(); });
                    UIUtil.addTrigger(() => hoverSound.Play(), EventTriggerType.PointerEnter, button, gameObject);
                    break;
                case RECORDS_NAME:
                    button.onClick.AddListener(delegate () { recordsButtonClicked(); });
                    UIUtil.addTrigger(() => hoverSound.Play(), EventTriggerType.PointerEnter, button, gameObject);
                    break;
                case END_GAME_BUTTON:
                    button.onClick.AddListener(delegate () { endGameButtonClicked(); });
                    UIUtil.addTrigger(() => hoverSound.Play(), EventTriggerType.PointerEnter, button, gameObject);
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

            LoadLevel(AppConfig.MENU_TRACK);
        }
    }

    private void howToPlayButtonClicked()
    {
        if (startBlocked) return;

        selectSound.Play();
        startBlocked = true;

        LoadLevel(AppConfig.MENU_HOWTOPLAY);
    }

    private void recordsButtonClicked()
    {
        if (startBlocked) return;

        selectSound.Play();
        startBlocked = true;

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
