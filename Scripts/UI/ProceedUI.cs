using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using UnityEngine.SceneManagement;

public class ProceedUI : PausableBehaviour {

    private Button next;
    private Button exitGame;
    private Text nextText;
    private Text exitText;
    private AsyncOperation async = null;

    private bool loadingBlocked = false;
    private bool appearing = false;
    private float startTime = 0f;
    private float endTime = 0.3f;

    public AudioObject hoverSound;

    public GameData gameData;

    public SceneFade sceneFade;

	protected override void _awake() {
        next = transform.Find("Next").GetComponent<Button>();
        exitGame = transform.Find("Exit").GetComponent<Button>();
        nextText = next.transform.Find("Text").GetComponent<Text>();
        exitText = exitGame.transform.Find("Text").GetComponent<Text>();

        next.onClick.AddListener(delegate () { goToNext(); });
        UIUtil.addTrigger(() => hoverSound.Play(), EventTriggerType.PointerEnter, next, gameObject);

        exitGame.onClick.AddListener(delegate () { returnToMain(); });
        UIUtil.addTrigger(() => hoverSound.Play(), EventTriggerType.PointerEnter, exitGame, gameObject);

        gameObject.SetActive(false);
    }


    private void returnToMain()
    {
        if (loadingBlocked) return;
        loadingBlocked = true;

        sceneFade.fade();
        callAfterSeconds(sceneFade.duration, () =>
        {
            LoadLevel("MainMenu");
        });
    }

    public void AppearAfterSeconds (float seconds) {
        gameObject.SetActive(true);
        updateColors();
        callAfterSeconds(seconds, () =>
        {
            appearing = true;
            startTime = pauseInvariantTime;
        });
	}

    protected override void _update()
    {
        if (appearing)
        {
            updateColors();
        }
    }

    private void updateColors()
    {
        Color fadeColor = next.GetComponent<Image>().color;
        float alpha = getAlpha();
        fadeColor.a = alpha;
        next.GetComponent<Image>().color = fadeColor;
        exitGame.GetComponent<Image>().color = fadeColor;

        Color letterColor = nextText.color;
        letterColor.a = alpha;
        nextText.color = letterColor;
        exitText.color = letterColor;
    }

    private float getAlpha()
    {
        if (appearing)
        {
            return Mathf.Lerp(0, 1, (Time.fixedTime - startTime) / endTime);
        }
        else
        {
            return 0f;
        }
    }

    private void goToNext()
    {
        if (loadingBlocked) return;
        loadingBlocked = true;

        sceneFade.fade();
        callAfterSeconds(sceneFade.duration, () =>
        {
            gameData.loadNextTrack();
        });
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
