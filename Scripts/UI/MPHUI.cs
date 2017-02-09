using UnityEngine;
using UnityEngine.UI;

public class MPHUI : PausableBehaviour
{
    private const string _MPH = " MPH";

    public Color maxColor = Color.red;
    public Color midColor = Color.yellow;
    public Color minColor = Color.white;

    public RacePlayer player;

    private Text textComponent;

    protected override void _awake()
    {
        textComponent = GetComponent<Text>();
        transform.position = new Vector3(70f, 20f, Camera.main.nearClipPlane);
    }

    protected override void _update()
    {
        textComponent.text = ((int)player.speed) + _MPH;
        textComponent.color = getColor();
    }

    private Color getColor()
    {
        if (player.speed < player.fwd_max_speed)
        {
            return Color.Lerp(minColor, midColor, player.speed / player.fwd_max_speed);
        }
        else
        {
            return Color.Lerp(midColor, maxColor, (player.speed - player.fwd_max_speed) / player.fwd_max_speed);
        }
    }
}
