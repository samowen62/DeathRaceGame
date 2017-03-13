using UnityEngine;
using UnityEngine.UI;

public class StartingSequence : PausableBehaviour
{

    private const float scriptBegin = 2f;
    private const float scriptEnd = scriptBegin + 5f;

    private const float fontSmall = 120f;
    private const float fontLarge = 240f;

    private Text screenText;

    public bool finished { get; set; }
    public bool seq_finished { get; set; }


    protected override void _awake () {
        screenText = GetComponent<Text>();
        finished = false;
        seq_finished = false;
    }

    protected override void _update () {

        if (seq_finished) return;

        float progress = pauseInvariantTime;
        //only in sequence if in here
        if (progress > scriptBegin && progress < scriptEnd)
        {
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
            else if(progress < scriptBegin + 5f)
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
