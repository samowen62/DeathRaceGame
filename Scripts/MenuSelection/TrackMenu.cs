using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class TrackMenu : MonoBehaviour
{
    public AudioObject selectSound;

    public AudioObject hoverSound;

    //TODO: use array of buttons with enum class specifying string
    private Button track_1_button;
    private Button track_2_button;
    private Button backButton;

    private const string TEST_1 = "Test 1";
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
        if (!loadingBlocked)
        {
            loadingBlocked = true;
            Debug.Log("Started Test Track sequence");

            SyncLoadLevel(AppConfig.TRACK_PAPER_ENGINE);
        }
    }

    private void load_test_2()
    {
        if (!loadingBlocked)
        {
            loadingBlocked = true;
            Debug.Log("Started Test Track sequence");

            SyncLoadLevel(AppConfig.TRACK_STAIRS);
        }
    }


    private void backButtonClicked()
    {
        if (!loadingBlocked)
        {
            loadingBlocked = true;
            Debug.Log("Started Test Track sequence");

            SyncLoadLevel(AppConfig.MENU_MAIN);
        }
    }

    private void SyncLoadLevel(string levelName)
    {
        async = SceneManager.LoadSceneAsync(levelName);
        StartCoroutine(Load());
    }

    IEnumerator Load()
    {
        Debug.Log("progress: " + async.progress);
        yield return async;
    }
}
