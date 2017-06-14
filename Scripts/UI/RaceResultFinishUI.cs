using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RaceResultFinishUI : MonoBehaviour {

    private Text racerText;
    private bool started = false;
    private float timeStarted;
    private Vector3 initialPosition;
    private Vector3 finalPosition;

    public float totalTime = 0.8f;
    public float ending_x_pos = 40f;

    private void Awake()
    {
        racerText = transform.Find("PlacementDisplayText").GetComponent<Text>();
    }

    public void startAnimation(float seconds, string text, int placement, int ending_y_pos)
    {
        racerText.text = placement + " " + text;//TODO: maybe spruce this up        
        finalPosition = new Vector3(ending_x_pos, ending_y_pos, 0);
        StartCoroutine(_startAnimation(seconds));
    }

    private IEnumerator _startAnimation(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        started = true;
        timeStarted = Time.fixedTime;
        initialPosition = transform.position;
    }

    void Update () {
        if (started && Time.fixedTime - timeStarted < totalTime + 0.05f)
        {
            transform.localPosition = Vector3.Lerp(initialPosition, finalPosition,
                (Time.fixedTime - timeStarted) / totalTime);
        } else
        {
            started = false;
        }
    }
}
