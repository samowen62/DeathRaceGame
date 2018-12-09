using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using System.Linq;

public class SelectTrackMenu : MonoBehaviour {

    public AudioObject selectSound;
    public AudioObject hoverSound;
    public GameData gameData;
    public Button backButton;

    private const string BACK_BUTTON = "Back";
    private bool loadingBlocked;
    private AsyncOperation async = null;

    // Use this for initialization
    void Start()
    {
        Debug.Log("Death Race Game Start!!");
        loadingBlocked = false;

        backButton.onClick.AddListener(delegate () { backButtonClicked(); });
        UIUtil.addTrigger(() => hoverSound.Play(), EventTriggerType.PointerEnter, backButton, gameObject);

        var trackNames = AppConfig.trackNameMap.Keys.ToList();
        var buttons = FindObjectsOfType<SelectTrackOption>();

        foreach (var button in buttons)
        {
            var btnName = button.name;

            //NOTE: must name the buttons after tracks for this to work
            if (trackNames.Contains(btnName))
            {
                button.Click += () => loadTrack(btnName);
                button.MouseEnter += () => hoverSound.Play();
            }
        }
    }

    private void loadTrack(string trackName)
    {
        if (loadingBlocked) return;
        loadingBlocked = true;

        // get the singleton reference rather than this object's
        var gameDataSingleton = GameData.Instance ?? gameData;
        gameDataSingleton.ReInitialize(trackName);
        DontDestroyOnLoad(gameDataSingleton);
        gameDataSingleton.loadSceneAfterSeconds("RacerMenu", 0.0f);
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
