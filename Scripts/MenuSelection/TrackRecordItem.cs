using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackRecordItem : MonoBehaviour {

    private Text _trackName;
    private Text _bestOverallRacer;
    private Text _bestLapRacer;

    private bool _loaded = false;
	private void load () {
        _trackName = GameObject.Find("Track/Text").GetComponent<Text>();
        _bestLapRacer = GameObject.Find("BestLap/Text").GetComponent<Text>();
        _bestOverallRacer = GameObject.Find("BestOverall/Text").GetComponent<Text>();
        _loaded = true;
    }

    public void updateWith(string trackName, SavedData.TrackRecord trackData)
    {
        if (!_loaded)
        {
            load();
        }
        _trackName.text = AppConfig.getTrackDisplayName(trackName);
        _bestLapRacer.text = trackData.BestLapTimeRacerName;
        _bestOverallRacer.text = trackData.BestTotalTimeRacerName;
        //TODO:images for racers
    }
}
