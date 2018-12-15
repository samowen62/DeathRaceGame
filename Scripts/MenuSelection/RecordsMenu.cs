using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RecordsMenu : MonoBehaviour
{

    public AudioObject hoverSound;

    [Serializable]
    public struct RacerPicture
    {
        public string racerName;
        public Texture2D racerImage;
    }

    public RacerPicture[] racerPictures;
    private Dictionary<string, Texture2D> racerPictureMap = new Dictionary<string, Texture2D>();

    private GameObject _noRecordsDisplay;
    private TrackRecordItem _recordTemplate;
    private Button _backButton;
    private AsyncOperation async = null;

    // Use this for initialization
    void Awake () {
        _backButton = GameObject.Find("BackButton").GetComponent<Button>();
        _noRecordsDisplay = GameObject.Find("NoRecords");
        _backButton.onClick.AddListener(delegate () { backButtonClicked(); });
        UIUtil.addTrigger(() => hoverSound.Play(), EventTriggerType.PointerEnter, _backButton, gameObject);

        _recordTemplate = GameObject.Find("ScrollRect/RecordContainer/Record").GetComponent<TrackRecordItem>();
        var records = DataLoader.LoadSavedData();

        // map of player names to pictures
        foreach (var picture in racerPictures)
        {
            racerPictureMap.Add(picture.racerName, picture.racerImage);
        }


        // no records to display
        if(records.TrackRecords.Count < 1)
        {
            Destroy(GameObject.Find("ScrollRect/RecordContainer/Record"));
            return;
        } else
        {
            _noRecordsDisplay.SetActive(false);
        }

        if(records.TrackRecords.Count < 3)
        {
            GameObject.Find("ScrollRect").GetComponent<ScrollRect>().vertical = false;
        }


        int i = 0;
        foreach (KeyValuePair<string, SavedData.TrackRecord> record in records.TrackRecords)
        {
            if(i == 0)
            {
                _recordTemplate.UpdateWith(record.Key, record.Value,
                    racerPictureMap[record.Value.BestTotalTimeRacerName], racerPictureMap[record.Value.BestLapTimeRacerName]);
            }
            else
            {
                addTrackRecordItem(record.Key, record.Value, 250 + i * -100);
            }

            i++;
        }

    }

    private void addTrackRecordItem(string trackName, SavedData.TrackRecord trackRecord, int downwardDistance)
    {
        TrackRecordItem newRecordItem = Instantiate(
            _recordTemplate,
            _recordTemplate.transform.position,
            Quaternion.identity) as TrackRecordItem;
        newRecordItem.transform.parent = _recordTemplate.transform.parent;
        newRecordItem.transform.localPosition = new Vector3(0, downwardDistance, 0);
        newRecordItem.transform.localScale = new Vector3(1, 1, 1);
        newRecordItem.transform.localRotation = Quaternion.identity;
        newRecordItem.UpdateWith(trackName, trackRecord,
            racerPictureMap[trackRecord.BestTotalTimeRacerName], racerPictureMap[trackRecord.BestLapTimeRacerName]);
    }

    private void backButtonClicked()
    {
        Debug.Log("Back to main");
        LoadLevel(AppConfig.MENU_MAIN);
    }

    private void LoadLevel(string levelName)
    {
        this.SyncLoadLevel(levelName);
    }
}
