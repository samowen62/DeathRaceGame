using UnityEngine.UI;
using UnityEngine;

public class CenterTextUI : PausableBehaviour
{
    private CenterText status;

    private Text text;
    private Image image;

    public RacePlayer player;
    public Color finishColor;
    public Color deathColor;

    protected override void _awake () {
        text = transform.Find("CenterText").GetComponent<Text>();
        image = GetComponent<Image>();

        status = CenterText.NONE;
        updateText();
    }

    protected override void _update () {
        
        //order of these is important
        if(player.finished)
        {
            status = CenterText.FINISHED;
            updateText();
        } else if(player.isDead)
        {
            status = CenterText.DEATH;
            updateText();
        }
        else if(status != CenterText.NONE)
        {
            status = CenterText.NONE;
            updateText();
        }
	}

    protected override void onPause()
    {
        text.text = "";
        image.enabled = false;
    }

    protected override void onUnPause()
    {
        updateText();
    }

    private void updateText()
    {
        switch (status)
        {
            case CenterText.NONE:
                text.text = "";
                image.enabled = false;
                break;
            case CenterText.FINISHED:
                text.text = "FINISH";
                text.color = finishColor;
                image.enabled = true;
                break;
            case CenterText.DEATH:
                text.text = "DEATH";
                text.color = deathColor;
                image.enabled = false;
                break;
        }
    }
}
