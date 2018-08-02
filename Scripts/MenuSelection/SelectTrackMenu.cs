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
    private bool loadingBlocked = false;
    private AsyncOperation async = null;

    // Use this for initialization
    void Start()
    {
        Debug.Log("Death Race Game Start!!");
        
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
        loadingBlocked = true;
        gameData.ReInitialize(trackName);
        DontDestroyOnLoad(gameData);
        gameData.loadSceneAfterSeconds("RacerMenu", 0.0f);
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
