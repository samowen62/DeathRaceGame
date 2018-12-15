using System;
using System.Collections.Generic;

[Serializable]
public class SavedData {

    public SavedData()
    {
        this.TrackRecords = new Dictionary<string, TrackRecord>();
    }

    // map of track names to track records
	public Dictionary<string, TrackRecord> TrackRecords { get; set; }

    [Serializable]
    public class TrackRecord
    {
        public float? BestLapTime { get; set; }
        public float? BestTotalTime { get; set; }
        public string BestLapTimeRacerName { get; set; }
        public string BestTotalTimeRacerName { get; set; }
    }
}
