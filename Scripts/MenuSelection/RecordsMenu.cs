using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RecordsMenu : MonoBehaviour
{

    public AudioObject hoverSound;

    private TrackRecordItem _recordTemplate;
    private Button _backButton;
    private AsyncOperation async = null;

    // Use this for initialization
    void Awake () {
        _backButton = GameObject.Find("BackButton").GetComponent<Button>();
        _backButton.onClick.AddListener(delegate () { backButtonClicked(); });
        UIUtil.addTrigger(() => hoverSound.Play(), EventTriggerType.PointerEnter, _backButton, gameObject);

        _recordTemplate = GameObject.Find("ScrollRect/RecordContainer/Record").GetComponent<TrackRecordItem>();
        var records = DataLoader.LoadSavedData();

        //TODO:test this (disable the Record)
        if(records.TrackRecords.Count < 1)
        {
            Destroy(_recordTemplate);
            return;
        }
        Debug.Log(records.TrackRecords.Count + " Records");
        //TODO:if too few disable scroll

        int i = 0;
        foreach (var record in records.TrackRecords)
        {
            if(i == 0)
            {
                _recordTemplate.updateWith(record.Key, record.Value);
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
        newRecordItem.updateWith(trackName, trackRecord);
    }

    private void backButtonClicked()
    {
        Debug.Log("Back to main");
        SyncLoadLevel(AppConfig.MENU_MAIN);
    }

    //TODO:put both of these in a util method
    private void SyncLoadLevel(string levelName)
    {
        StartCoroutine(Load(levelName));
    }

    IEnumerator Load(string levelName)
    {
        async = SceneManager.LoadSceneAsync(levelName);
        yield return async;
    }
}
