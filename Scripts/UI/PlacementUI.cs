using UnityEngine;
using UnityEngine.UI;

public class PlacementUI : PausableBehaviour
{

    public RacePlayer player;

    private Text textComponent;

    protected override void _awake()
    {
        textComponent = GetComponent<Text>();
        transform.position = new Vector3(Screen.width, 30f, Camera.main.nearClipPlane);
    }

    protected override void _update()
    {
        textComponent.text = getPlacementString(player.placement);
    }

    private string getPlacementString(int placement)
    {
        switch (placement)
        {
            case 1:
                return "1st";
            case 2:
                return "2nd";
            case 3:
                return "3rd";
            case 4:
                return "4th";
            case 5:
                return "5th";
            case 6:
                return "6th";
            case 7:
                return "7th";
            case 8:
                return "8th";
            default:
                return "?? :O";
        }
    }
}
