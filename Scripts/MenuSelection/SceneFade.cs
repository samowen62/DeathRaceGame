using UnityEngine;
using UnityEngine.UI;

public class SceneFade : PausableBehaviour {

    private float startTime = 0;
    public float endTime = 0.5f;
    private bool started = false;

    public float duration { get { return endTime; } }
    private Image fadeToBlack;

    protected override void _awake () {
        fadeToBlack = GetComponent<Image>();
    }

    protected override void _update()
    {
        if (started)
        {
            Color fadeColor = fadeToBlack.color;
            fadeColor.a = Mathf.Lerp(0, 1, (Time.fixedTime - startTime) / endTime);
            fadeToBlack.color = fadeColor;
        }
    }

    public void fade()
    {
        startTime = Time.fixedTime;
        started = true;

        fadeToBlack.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
        fadeToBlack.transform.position = new Vector3(Screen.width / 2, Screen.height / 2);
    }
}
