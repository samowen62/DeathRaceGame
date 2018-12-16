using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RacerMenu : MonoBehaviour {

    public GameData gameData;

    public AudioObject hoverSound;

    public AudioObject selectSound;

    public SceneFade sceneFade;

    public OptionHover proceedButton;

    private RacerOption selectedRacer = null;

    private AsyncOperation async = null;

    private const string PLAYER_1 = "Player 1";
    private const string PLAYER_2 = "Player 2";
    private const string PLAYER_3 = "Player 3";
    private const string PLAYER_4 = "Player 4";

    private const string PROCEED = "Proceed";
    private const string BACK_BUTTON = "BackButton";

    private bool loadingBlocked = false;

    // Use this for initialization
    void Start () {
        gameData = FindObjectOfType<GameData>();

        Button[] buttons = FindObjectsOfType<Button>();

        proceedButton.disable();

        foreach (var button in buttons)
        {
            switch (button.name)
            {
                case PLAYER_1:
                case PLAYER_2:
                case PLAYER_3:
                case PLAYER_4:
                    button.onClick.AddListener(() => chooseRacer(button));
                    //UIUtil.addTrigger(() => hoverSound.Play(), EventTriggerType.PointerEnter, button, gameObject);
                    break;

                case BACK_BUTTON:
                    button.onClick.AddListener(delegate () { backButtonClicked(); });
                    UIUtil.addTrigger(() => hoverSound.Play(), EventTriggerType.PointerEnter, button, gameObject);

                    break;
                case PROCEED:
                    button.onClick.AddListener(delegate () { proceedButtonClicked(); });
                    UIUtil.addTrigger(() => hoverSound.Play(), EventTriggerType.PointerEnter, button, gameObject);

                    break;
                default:
                    Debug.LogWarning("unused button component: " + button.name);
                    break;
            }
        }
    }

    private void chooseRacer( Button button)
    {
        if (selectedRacer == null || loadingBlocked) selectedRacer.deselect();

        selectedRacer = button.GetComponent<RacerOption>();
        selectedRacer.select();
        proceedButton.enable();
    }

    private void proceedButtonClicked()
    {
        if (selectedRacer == null || loadingBlocked) return;

        loadingBlocked = true;
        gameData.mainPlayer = selectedRacer.name;
        DontDestroyOnLoad(gameData);
        sceneFade.fade();
        gameData.loadSceneAfterSeconds(gameData.sceneSequence[0], sceneFade.duration);
    }


    private void backButtonClicked()
    {
        if (!loadingBlocked)
        {
            loadingBlocked = true;
            Debug.Log("Back to main");

            LoadLevel(AppConfig.MENU_MAIN);
        }
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
