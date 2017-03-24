public class DataDTO
{
    public int[] placements;

    public float[] lapTimes;

    public DataDTO(int tracks)
    {
        placements = new int[tracks];
        lapTimes = new float[tracks];
    }
}