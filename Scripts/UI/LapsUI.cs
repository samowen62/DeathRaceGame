using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LapsUI : PausableBehaviour {

    public GameContext context;

    public RacePlayer player;

    private Text textComponent;
    private PlacementDTO placement;

    private const string LAP = "Lap ";
    private const string CURRENT_LAP = "Current: ";
    private const string COLON = ": ";
    private const string NEW_LINE = "\n";
    private const string FORMAT = "{0:0.##}";

    protected override void _awake () {
        textComponent = GetComponent<Text>();
    }

    protected override void _update () {
        if(placement == null)
        {
            if (context.racerPlacement != null)
            {
                placement = context.racerPlacement[player];
            }else
            {
                return;
            }
        }

        string text = "";

        int laps = Mathf.Min(context.laps, placement.lap);
        for (int i = 0; i < placement.lap - 1; i++)
        {
            text += LAP + (placement.lap - 1 - i) + COLON +
                string.Format(FORMAT, placement.lapTimes[placement.lap - 2 - i]) + NEW_LINE; 
        }

        if (!player.finished && placement.lap <= placement.lapStart.Length)
        {
            text += CURRENT_LAP + string.Format(FORMAT, pauseInvariantTime - placement.lapStart[placement.lap - 1]);
        }
        textComponent.text = text;
    }
}
