using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TrackMenu : MonoBehaviour
{
    public AudioObject selectSound;

    public AudioObject hoverSound;

    public GameData gameData;
    
    private Button track_1_button;
    private Button track_2_button;
    private Button backButton;

    private const string TEST_1 = "Curcuit 1";
    private const string TEST_2 = "Test 2";
    private const string BACK_BUTTON = "Back";

    private bool loadingBlocked = false;
    private AsyncOperation async = null;

    // Use this for initialization
    void Start()
    {
        Debug.Log("Death Race Game Start!!");
        Button[] buttons = FindObjectsOfType<Button>();

        foreach (var button in buttons)
        {
            switch (button.name)
            {
                case TEST_1:
                    track_1_button = button;
                    track_1_button.onClick.AddListener(delegate () { load_test_1(); });
                    UIUtil.addTrigger(() => hoverSound.Play(), EventTriggerType.PointerEnter, track_1_button, gameObject);

                    break;
                case TEST_2:
                    track_2_button = button;
                    track_2_button.onClick.AddListener(delegate () { load_test_2(); });
                    UIUtil.addTrigger(() => hoverSound.Play(), EventTriggerType.PointerEnter, track_2_button, gameObject);

                    break;
                case BACK_BUTTON:
                    backButton = button;
                    backButton.onClick.AddListener(delegate () { backButtonClicked(); });
                    UIUtil.addTrigger(() => hoverSound.Play(), EventTriggerType.PointerEnter, backButton, gameObject);

                    break;
                default:
                    Debug.LogWarning("unused button component: " + button.name);
                    break;
            }
        }


    }

    private void load_test_1()
    {
        DontDestroyOnLoad(gameData);
        gameData.loadSceneAfterSeconds("RacerMenu", 0.0f);
    }

    private void load_test_2()
    {
        if (!loadingBlocked)
        {
            loadingBlocked = true;
            Debug.Log("Started Test Track sequence");

            LoadLevel(AppConfig.TRACK_STAIRS);
        }
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
        this.SyncLoadLevel(levelName);
    }
}
