using UnityEngine;

public class PlacementDTO  {

    public int lap;
    public int latestTrackPoint;

    public float[] lapTimes;
    public float[] lapStart;

    public PlacementDTO(int laps)
    {
        lap = 1;
        latestTrackPoint = -1;
        lapTimes = new float[laps];
        lapStart = new float[laps];
    }

}
