using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlacementFinishUI : PausableBehaviour {

    private Text playerText;
    private Text lapTimeText;
    private Text placementNumber;
    private Image image;

    private string playerTxt;
    private string lapTimeTxt;
    private string placementNumberTxt;

    private Vector3 initialPosition;
    private Vector3 finalPosition;

    private float totalTime = 0.7f;
    private float waitTime = 3f;
    public float waitingTime { get { return waitTime; } }
    private float timeStarted;
    private bool started = false;

    protected override void _awake () {
        playerText = transform.Find("PlayerText").gameObject.GetComponent<Text>();
        lapTimeText = transform.Find("LapTimeText").gameObject.GetComponent<Text>();
        placementNumber = transform.Find("PlacementNumber").gameObject.GetComponent<Text>();
        image = GetComponent<Image>();
        clear();
    }

    public void setPrams(string _playerTxt, string _lapTimeTxt, string _placementNumberTxt, float _downwardDistance)
    {
        playerTxt = _playerTxt;
        lapTimeTxt = _lapTimeTxt;
        placementNumberTxt = _placementNumberTxt;
        initialPosition = transform.position;
        finalPosition = initialPosition + new Vector3(0, _downwardDistance, 0);
        gameObject.SetActive(true);
    }

    public void startAnimation()
    {
        callAfterSeconds(waitTime, () => {
            display();
            started = true;
            timeStarted = pauseInvariantTime;
        });
    }

    protected override void _update () {
	
        if(pauseInvariantTime - timeStarted < totalTime && started)
        {
            transform.position = Vector3.Lerp(initialPosition, finalPosition, 
                (pauseInvariantTime - timeStarted) / totalTime);
        }

	}

    protected override void onPause()
    {
        clear();
    }

    protected override void onUnPause()
    {
        display();
    }

    private void display()
    {
        playerText.text = playerTxt;
        lapTimeText.text = lapTimeTxt;
        placementNumber.text = placementNumberTxt;
        image.enabled = true;
    }

    private void clear()
    {
        playerText.text = "";
        lapTimeText.text = "";
        placementNumber.text = "";
        image.enabled = false;
    }
}
