using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class LapsUI : PausableBehaviour {

    public PlacementManager placementManager;

    public RacePlayer player;

    private Text textComponent;

    private const string LAP = "Lap ";
    private const string CURRENT_LAP = "Current: ";
    private const string COLON = ": ";
    private const string NEW_LINE = "\n";

    protected override void _awake () {
        textComponent = GetComponent<Text>();
        textComponent.text = "";
    }

    protected override void _update () {

        if (!player.startedLap1) return;

        string text = "";
        float[] lapTimes = placementManager.getLapTimesForPlayer(player);
        float lastLapStart = placementManager.getLastLapStart(player);
        
        for(int i = lapTimes.Length - 1; i >= 0; i--)
        {
            if (lapTimes[i] != 0f)
            {
                text += LAP + (i + 1) + COLON +
                    formatSecondsToTime(lapTimes[i]) + NEW_LINE;
            }
        }

        if (!player.finished)
        {
            text += CURRENT_LAP + formatSecondsToTime(pauseInvariantTime - lastLapStart);
        }

        textComponent.text = text;
    }

    private string formatSecondsToTime(float seconds)
    {
        var span = TimeSpan.FromSeconds(seconds);
        return span.Minutes.ToString("00") + ":" +
            span.Seconds.ToString("00") + "." +
            span.Milliseconds.ToString("00");
    }
}
