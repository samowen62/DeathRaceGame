using UnityEngine;
using UnityEngine.UI;

public class HealthUI : PausableBehaviour
{

    private const string _HEALTH = "HEALTH: ";
    private const int barLength = 120;

    public Color maxColor = Color.blue;
    public Color midColor = Color.green;
    public Color minColor = Color.red;

    private Color current_color;
    private Vector3 current_scale = Vector3.one;

    public RacePlayer player;

    private Text textComponent;
    private Image imageComponent;
    private Transform scaleComponent;

    protected override void _awake()
    {
        textComponent = transform.Find("HealthDisplay").GetComponent<Text>();
        scaleComponent = transform.Find("ImageScale");
        imageComponent = scaleComponent.Find("HealthBarImage").GetComponent<Image>();
    }

    protected override void _update()
    {
        current_color = getColor();
        textComponent.text = _HEALTH + ((int)player.health);
        textComponent.color = current_color;
        imageComponent.color = current_color;
        current_scale.x = player.health / player.starting_health;
        scaleComponent.localScale = current_scale;
    }

    private Color getColor()
    {
        if (player.health <= player.starting_health)
        {
            return Color.Lerp(minColor, midColor, player.health / player.starting_health);
        }
        else
        {
            return Color.Lerp(midColor, maxColor, (player.health - player.starting_health) / (player.max_bonus_health - player.starting_health));
        }
    }
}
