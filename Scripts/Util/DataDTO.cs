using System.Collections.Generic;

public class DataDTO
{
    public int[] placements;

    //TODO:use lapTimes for multiple laps
    public List<float>[] lapTimes;

    public DataDTO(int tracks)
    {
        placements = new int[tracks];
        lapTimes = new List<float>[tracks];
    }
}