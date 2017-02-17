using UnityEngine;
using System.Collections;

public static class AppConfig {

    public static int groundMask = 1 << 8;
    public static int wallMask = 1 << 11;
    //delete??
    public static int trackPointMask = 1 << 12;

    public static string MENU_MAIN = "MainMenu";
    public static string MENU_TRACK = "TrackMenu";

    public static string TRACK_PAPER_ENGINE = "paperEngine";
    public static string TRACK_STAIRS = "StairsTrack";


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
