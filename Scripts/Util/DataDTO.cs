public class DataDTO
{
    public int[] placements;

    //TODO:use lapTimes for multiple laps
    public float[] lapTimes;

    public DataDTO(int tracks)
    {
        placements = new int[tracks];
        lapTimes = new float[tracks];
    }
}