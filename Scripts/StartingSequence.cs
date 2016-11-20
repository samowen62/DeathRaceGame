using UnityEngine;
using UnityEngine.UI;

public class StartingSequence : MonoBehaviour {

    private float timeProgression;
    private const float second = 1f;
    private const float scriptBegin = 2f;

    private float timePaused;
    private Text screenText;

    public bool finished { get; set; }

    private bool _behaviorBlocked;
    public bool behaviorBlocked
    {
        get
        {
            return _behaviorBlocked;
        }
        set
        {
            if (value)
            {
                timePaused = Time.fixedTime;
            }
            else
            {
                //this is to ensure that pausing the game does not mess with timing
                timeProgression += (Time.fixedTime - timePaused);
                _behaviorBlocked = value;
            }
        }
    }

    // Use this for initialization
    void Awake () {
        screenText = GetComponent<Text>();
        timeProgression = Time.fixedTime;
        finished = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (behaviorBlocked)
        {
            return;
        }

        float progress = Time.fixedTime - timeProgression;

        //only in sequence if in here
        if(progress > 2f && progress < 7f)
        {
            if(progress < 3f)
            {
                screenText.text = "3";
            } else if (progress < 4f)
            {
                screenText.text = "2";
            }
            else if (progress < 5f)
            {
                screenText.text = "1";
            }
            else if (progress < 6f)
            {
                screenText.text = "GO!";
                finished = true;
            }
            else
            {
                screenText.text = "";
            }
        }

    }
}
