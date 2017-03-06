using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{

    public GameContext context;

    public AudioObject selectSound;

    public AudioObject hoverSound;

    //TODO: use array of buttons with enum class specifying string
    private Button resumeButton;
    private Button exitGameButton;

    private const string RESUME_GAME = "Resume";
    private const string EXIT_GAME = "Exit Game";

    private bool loadingBlocked = false;
    private AsyncOperation async = null;

    // Use this for initialization
    void Start()
    {
        if (context == null)
        {
            Debug.LogError("Please add a GameContext object to this instance of PauseMenu");
        }

        Button[] buttons = FindObjectsOfType<Button>();

        foreach (var button in buttons)
        {
            switch (button.name)
            {
                case RESUME_GAME:
                    resumeButton = button;
                    resumeButton.onClick.AddListener(delegate () { resumeGame(); });
                    UIUtil.addTrigger(() => hoverSound.Play(), EventTriggerType.PointerEnter, resumeButton, gameObject);
                    break;
                case EXIT_GAME:
                    exitGameButton = button;
                    exitGameButton.onClick.AddListener(delegate () { exitGame(); });
                    UIUtil.addTrigger(() => hoverSound.Play(), EventTriggerType.PointerEnter, exitGameButton, gameObject);

                    break;
                default:
                    Debug.LogWarning("unused button component: " + button.name);
                    break;
            }
        }


    }

    private void resumeGame()
    {
        context.unpauseGame();
    }

    private void exitGame()
    {
        selectSound.Play();
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
