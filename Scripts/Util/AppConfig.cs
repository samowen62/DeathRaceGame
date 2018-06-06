using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class AppConfig {

    public static readonly int groundMask = 1 << 8;
    public static readonly int wallMask = 1 << 11;

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
        myLine.name = "Line";
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.SetColors(color, color);
        lr.SetWidth(2f, 2f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }

    public static void changeParent(GameObject newParent, GameObject child)
    {
        Vector3 oldTrans = child.transform.localPosition;
        Vector3 oldScale = child.transform.localScale;
        Quaternion oldRotation = child.transform.localRotation;
        child.transform.SetParent(newParent.transform);
        child.transform.localPosition = oldTrans;
        child.transform.localRotation = oldRotation;
        child.transform.localScale = oldScale;
    }

    public static T findOnly<T>()
    {
        var obj = Object.FindObjectsOfType(typeof(T)) as T[];
        if (obj.Length != 1)
        {
            Debug.LogError("Error: only 1 component of type '" + typeof(T) + "' allowed in the scene");
            return default(T);
        }
        return obj[0];
    }

    /* Map of player names to display names. Used below */
    public static Dictionary<string, string> displayNameMap = new Dictionary<string, string>()
    {
        {"Player 1", "Sharp Ship" },
        {"Player 2", "Red Racer" },
        {"Player 3", "Green Machine" },
        {"Player 4", "Penetrator" }
    };

    /* Map of track names to display names*/
    public static Dictionary<string, string> trackNameMap = new Dictionary<string, string>()
    {
        {"HalfPipe", "Track 1" },
        {"SimpleTrack", "Track 2" },
        {"paperEngine", "Track 3" },
        {"SnowTrack", "Track 4" },
        {"SpiralTrack", "Track 5" }
    };

    /**
     * gets the racer's display name given the internally used game 
     * (i.e.) "Player 1" -> "Sharp Racer"
     */
    public static string getRacerDisplayName(string playername)
    {
        string displayName = "No displayName!";
        if (displayNameMap.TryGetValue(playername, out displayName))
        {
            return displayName;
        }

        Debug.Log("no display name found for player: " + playername + "!!");
        return displayName;
    }
}
