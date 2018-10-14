using UnityEngine;
using UnityEngine.UI;

public class HowToPlayMenu : MonoBehaviour
{
    private Button _backButton;
    private AsyncOperation async = null;
    
    void Awake()
    {
        _backButton = GameObject.Find("BackButton").GetComponent<Button>();
        _backButton.onClick.AddListener(delegate () { backButtonClicked(); });
    }

    private void backButtonClicked()
    {
        Debug.Log("Back to main");
        LoadLevel(AppConfig.MENU_MAIN);
    }

    private void LoadLevel(string levelName)
    {
        this.SyncLoadLevel(levelName);
    }
}
