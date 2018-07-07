using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackRecordItem : MonoBehaviour {

    private Text _trackName;
    private Text _bestOverallRacer;
    private Text _bestLapRacer;
    private Image _bestOverallRacerImage;
    private Image _bestLapRacerImage;

    private bool _loaded = false;
	private void load () {
        _trackName = GameObject.Find("Track/Text").GetComponent<Text>();
        _bestLapRacer = GameObject.Find("BestLap/Text").GetComponent<Text>();
        _bestOverallRacer = GameObject.Find("BestOverall/Text").GetComponent<Text>();
        _bestOverallRacerImage = GameObject.Find("BestOverallRacer").GetComponent<Image>();
        _bestLapRacerImage = GameObject.Find("BestLapRacer").GetComponent<Image>();
        _loaded = true;
    }

    public void UpdateWith(string trackName, SavedData.TrackRecord trackData, Texture2D bestOverallRacerImage, Texture2D bestLapRacerImage)
    {
        if (!_loaded)
        {
            load();
        }
        _trackName.text = AppConfig.getTrackDisplayName(trackName);
        _bestLapRacer.text = "Best Lap Time:\n" + AppConfig.formatSecondsToTime(trackData.BestLapTime ?? 0);
        _bestOverallRacer.text = "Best Overall Time:\n" + AppConfig.formatSecondsToTime(trackData.BestTotalTime ?? 0);

        if (bestOverallRacerImage != null)
        {
            _bestOverallRacerImage.sprite = Sprite.Create(bestOverallRacerImage, new Rect(0, 0, bestOverallRacerImage.width, bestOverallRacerImage.height), new Vector2(0.5f, 0.5f));
        }

        if (bestLapRacerImage != null)
        {
            _bestLapRacerImage.sprite = Sprite.Create(bestLapRacerImage, new Rect(0, 0, bestLapRacerImage.width, bestLapRacerImage.height), new Vector2(0.5f, 0.5f));
        }
    }
}
