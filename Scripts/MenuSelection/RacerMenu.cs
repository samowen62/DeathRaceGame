using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RacerMenu : MonoBehaviour {

    public GameData gameData;

    public AudioObject hoverSound;

    public AudioObject selectSound;

    public SceneFade sceneFade;

    private string selectedRacer = null;

    private AsyncOperation async = null;

    private Button proceedButton;
    private Button backButton;

    private const string PLAYER_1 = "Player 1";
    private const string PLAYER_2 = "Player 2";

    private const string PROCEED = "Proceed";
    private const string BACK_BUTTON = "BackButton";

    private bool loadingBlocked = false;

    // Use this for initialization
    void Start () {
        gameData = FindObjectOfType<GameData>();

        Button[] buttons = FindObjectsOfType<Button>();

        foreach (var button in buttons)
        {
            switch (button.name)
            {
                //TODO: on click with a player we should highlight or outline them
                case PLAYER_1:
                    button.onClick.AddListener(delegate () { selectedRacer = PLAYER_1; });
                    UIUtil.addTrigger(() => hoverSound.Play(), EventTriggerType.PointerEnter, button, gameObject);

                    break;
                case PLAYER_2:
                    button.onClick.AddListener(delegate () { selectedRacer = PLAYER_2; });
                    UIUtil.addTrigger(() => hoverSound.Play(), EventTriggerType.PointerEnter, button, gameObject);

                    break;

                case BACK_BUTTON:
                    proceedButton = button;
                    proceedButton.onClick.AddListener(delegate () { backButtonClicked(); });
                    UIUtil.addTrigger(() => hoverSound.Play(), EventTriggerType.PointerEnter, proceedButton, gameObject);

                    break;
                case PROCEED:
                    backButton = button;
                    backButton.onClick.AddListener(delegate () { proceedButtonClicked(); });
                    UIUtil.addTrigger(() => hoverSound.Play(), EventTriggerType.PointerEnter, backButton, gameObject);

                    break;
                default:
                    Debug.LogWarning("unused button component: " + button.name);
                    break;
            }
        }
    }


    private void proceedButtonClicked()
    {
        if (selectedRacer == null) return;

        gameData.mainPlayer = selectedRacer;
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

            SyncLoadLevel(AppConfig.MENU_MAIN);
        }
    }

    //TODO:put both of these in a util method
    private void SyncLoadLevel(string levelName)
    {
        StartCoroutine(Load(levelName));
    }

    IEnumerator Load(string levelName)
    {
        sceneFade.fade();
        yield return new WaitForSeconds(sceneFade.duration);
        async = SceneManager.LoadSceneAsync(levelName);
        yield return async;
    }
}
