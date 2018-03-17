using UnityEngine;
using UnityEngine.UI;


public class EventTextUI : PausableBehaviour
{
    private Text text;
    private float colorAlpha;

    private bool moving = false;
    private bool fading = false;

    private const float downwardBumpDistance = 20;
    private const float downwardMovementSpeed = 0.8f;
    private float goalDownwardDist = 0f;
    private float currentDownwardDist = 0f;

    protected override void _awake()
    {
        text = GetComponent<Text>();
        colorAlpha = text.color.a;
    }

    public void setText(string _text)
    {
        text.text = _text;
    }

    protected override void _update()
    {

        if (moving)
        {
            if(currentDownwardDist >= goalDownwardDist)
            {
                moving = false;
            }
            currentDownwardDist += downwardMovementSpeed;
            transform.position -= new Vector3(0, downwardMovementSpeed, 0);
        }
        else if(fading)
        {
            var newColor = text.color;
            colorAlpha -= 0.06f;
            if(colorAlpha <= 0)
            {
                Destroy(gameObject);
            }

            newColor.a = colorAlpha;
            text.color = newColor;
        }
    }

    public void bumpDown()
    {
        moving = true;
        goalDownwardDist += downwardBumpDistance;
    }

    public void fadeOut()
    {
        fading = true;
    }

    public void clear()
    {
        text.text = "";
    }
}
