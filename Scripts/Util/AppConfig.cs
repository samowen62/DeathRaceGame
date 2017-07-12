using UnityEngine;

public static class AppConfig {

    public static readonly int groundMask = 1 << 8;
    public static readonly int wallMask = 1 << 11;
    //delete??
    public static readonly int trackPointMask = 1 << 12;

    public static readonly string MENU_MAIN = "MainMenu";
    public static readonly string MENU_TRACK = "TrackMenu";

    public static readonly string TRACK_PAPER_ENGINE = "paperEngine";
    public static readonly string TRACK_STAIRS = "StairsTrack";

    public static readonly KeyCode PAUSE_BUTTON = KeyCode.F;
    public static readonly KeyCode BOOST_BUTTON = KeyCode.Q;
    public static readonly KeyCode SHARP_TURN_BUTTON = KeyCode.Space;
    public static readonly KeyCode FORWARD_BUTTON = KeyCode.W;
    public static readonly KeyCode ATTACK_BUTTON = KeyCode.E;

    public static float hoverHeight = 1.8f;

    public static void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        GameObject myLine = new GameObject();
        myLine.name = "Bezier Line";
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.SetColors(color, color);
        lr.SetWidth(2f, 2f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }
}
