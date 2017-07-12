using UnityEngine;
using UnityEngine.UI;

public class StartingSequence : PausableBehaviour
{

    private const float scriptBegin = 5f;
    private const float scriptEnd = scriptBegin + 5f;

    private const float fontSmall = 120f;
    private const float fontLarge = 240f;

    private Text screenText;

    public AudioObject boostSound;

    public BezierSpline cameraPath;

    public RacePlayer mainRacer;

    public bool finished { get; set; } 
    public bool seq_finished { get; set; }

    public float time {
        get {
            return pauseInvariantTime;
        }
        set { }
    }

    protected override void _awake () {
        screenText = GetComponent<Text>();
    }

    protected override void _update () {

        if (seq_finished) return;

        float progress = pauseInvariantTime;

        if(progress <= scriptBegin)
        {
            float t = 0.98f - progress / scriptBegin;
            Vector3 cameraPt = cameraPath.GetPoint(t);
            Camera.main.transform.position = cameraPt;

            Vector3 cameraDir = mainRacer.transform.position - cameraPt;
            cameraDir = Vector3.ProjectOnPlane(cameraDir.normalized, mainRacer.transform.up);
            Camera.main.transform.rotation = 
                Quaternion.LookRotation(cameraDir, mainRacer.transform.up) * mainRacer.cameraRotation;
        }
        else if (progress > scriptBegin && progress < scriptEnd)
        {
            //TODO:adjust timing on boost sound
            if (!boostSound.started)
            {
                Camera.main.transform.localPosition = mainRacer.playerToCamera;
                Camera.main.transform.localRotation = mainRacer.cameraRotation;
                boostSound.Play();
            }

            if(progress < scriptBegin + 1f)
            {
                screenText.text = "3";
                screenText.fontSize = fontSizeForSeconds(progress);
            } else if (progress < scriptBegin + 2f)
            {
                screenText.text = "2";
                screenText.fontSize = fontSizeForSeconds(progress);
            }
            else if (progress < scriptBegin + 3f)
            {
                screenText.text = "1";
                screenText.fontSize = fontSizeForSeconds(progress);
            }
            else if (progress < scriptBegin + 4f)
            {
                screenText.text = "GO!";
                screenText.fontSize = 180;
                finished = true;
            }
            else if(progress < scriptEnd)
            {
                screenText.text = "";
                seq_finished = true;
            }
        }

    }

    private int fontSizeForSeconds(float seconds)
    {
        float timeInSeq = seconds - scriptBegin;
        //chop off int part and make into abs() function
        timeInSeq = -2.0f * Mathf.Abs((timeInSeq - (int)timeInSeq) - 0.25f) + 0.5f;
        return (int)Mathf.Lerp(fontSmall, fontLarge, timeInSeq);
    }
}
