using UnityEngine;
using UnityEngine.UI;

public class HealthUI : PausableBehaviour
{

    private const string _HEALTH = "HEALTH: ";
    private const int barLength = 120;

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
        textComponent.fontSize = 10;
    }

    protected override void _update()
    {
        current_color = getColor();
        textComponent.text = _HEALTH;
        textComponent.color = current_color;
        imageComponent.color = current_color;
        current_scale.x = player.health / player.StartingHealth;
        scaleComponent.localScale = current_scale;
    }

    private Color getColor()
    {
        if (player.health <= player.StartingHealth)
        {
            //.25 to 0
            return new HSBColor(player.health / (4 * player.StartingHealth), 1, 1).ToColor();
        }
        else
        {
            //TODO: make .5 to .25 as it lowers
            return new HSBColor((player.health - player.StartingHealth) / (4 * (player.MaxBonusHealth - player.StartingHealth)) + 0.25f, 1, 1).ToColor();
        }
    }
}
