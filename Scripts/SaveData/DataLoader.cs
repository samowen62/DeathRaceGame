using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class DataLoader {

    //TODO: verify!
    /// <summary>
    /// Attempts to save this track record in the record books.
    /// Returns a bool if a record was set and data was saved.
    /// </summary>
    /// <param name="record"></param>
    /// <returns></returns>
    public static bool SaveBestTimeRecord(SavedData.TrackRecord record, string racerName, string trackName)
    {
        var didSaveData = false;
        var savedData = LoadSavedData() ?? new SavedData();

        //TODO: USE trackName instead!!!!
        if (savedData.TrackRecords.ContainsKey(trackName))
        {
            var currentRecord = savedData.TrackRecords[trackName];
            var dirty = false;

            if(currentRecord.BestLapTime == null || record.BestLapTime.Value < currentRecord.BestLapTime.Value)
            {
                //new best lap time
                currentRecord.BestLapTime = record.BestLapTime;
                currentRecord.BestLapTimeRacerName = racerName;
                dirty = true;
            }

            if (currentRecord.BestTotalTime == null || record.BestTotalTime.Value < currentRecord.BestTotalTime.Value)
            {
                //new total lap time
                currentRecord.BestTotalTime = record.BestTotalTime;
                currentRecord.BestTotalTimeRacerName = racerName;
                dirty = true;
            }

            if (dirty)
            {
                savedData.TrackRecords[trackName] = currentRecord;
                Save(savedData);
                didSaveData = true;
            }
        }
        else
        {
            record.BestTotalTimeRacerName = racerName;
            record.BestLapTimeRacerName = racerName;
            savedData.TrackRecords[trackName] = record;
            Save(savedData);
            didSaveData = true;
        }

        return didSaveData;
    }

    private static SavedData LoadSavedData()
    {
        if (File.Exists(Application.persistentDataPath + "/savedGameData.gd"))
        {
            var bf = new BinaryFormatter();
            var file = File.Open(Application.persistentDataPath + "/savedGameData.gd", FileMode.Open);
            var gameData = (SavedData)bf.Deserialize(file);
            file.Close();
            //TODO: handle IO errors here
            return gameData;
        }

        return null;
    }

    private static void Save(SavedData gameData)
    {
        var bf = new BinaryFormatter();
        var file = File.Create(Application.persistentDataPath + "/savedGameData.gd");
        bf.Serialize(file, gameData);
        file.Close();
    }
}
