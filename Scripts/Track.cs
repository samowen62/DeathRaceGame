using UnityEngine;
using System.Collections;
using System.Xml;

//TODO: use this class to store AI data
public class Track : MonoBehaviour {

    public CheckPoint startingCheckPoint;

    public string trackName;

    public Vector3 [] bezierPoints { private set; get; }

    //prefab of track
    private GameObject trackPrefab;

    private const string TRACK_DATA_ROOT = "/Prefabs/Tracks/";
    private const string TRACK_DATA_FILE_NAME = "/TrackData.xml";

    // Use this for initialization
    void Awake () {
        validateTrack();

        readTrackData();

        drawPath();
	}

    /**
     * Validates assumptions about track used later, namely:
     *      1) the child track prefab is named 'Track'
     *      2) this gameobject has a scale of 1 in all directions
     */
    private void validateTrack()
    {
        trackPrefab = transform.Find("Track").gameObject;

        if (trackPrefab == null)
        {
            Debug.LogError("Please name the track prefab 'Track' in this instance of Track.cs");
        }

        if(gameObject.transform.localScale.x != 1 || gameObject.transform.localScale.y != 1 || gameObject.transform.localScale.z != 1)
        {
            Debug.LogError("scale of track prefab must be 1 in all directions");
        }
    }

    /**
     * This reads the xml file associated with this track and grabs the bezier curve data
     *  - looks at the in the TrackData.xml file in the <trackName> folder in Prefabs/Tracks
     */
    private void readTrackData()
    {
        if(trackName == "" || trackName == null)
        {
            Debug.LogError("Must add trackName to track!");
        }

        XmlDocument doc = new XmlDocument();
        doc.Load(Application.dataPath + TRACK_DATA_ROOT + trackName + TRACK_DATA_FILE_NAME);
        var pointList = doc.GetElementsByTagName("Point");

        if(pointList.Count == 0)
        {
            Debug.LogError("Need to have point data in xml file!");
        }

        bezierPoints = new Vector3[pointList.Count];
        int i = 0;

        Vector3 trackScale = trackPrefab.transform.localScale;
        foreach (XmlNode point in pointList)
        {
            Vector3 b_point = new Vector3();
            b_point.x = System.Convert.ToSingle(point.ChildNodes.Item(0).InnerText);
            b_point.y = System.Convert.ToSingle(point.ChildNodes.Item(1).InnerText);
            b_point.z = -System.Convert.ToSingle(point.ChildNodes.Item(2).InnerText);

            /*
             * Reset rotation and scale in blender. For some reason 
             * importing from blender to unity is flawed and we need to: 
             *  1) Negate z
             *  2) Rotate the point 180 on the y axis          
             */
            b_point = Quaternion.AngleAxis(180,Vector3.up) * b_point;

            /*
             * This will transform the bezier points into the real track's 
             * coordinates. It makes 2 assumptions:
             *  1) No parents of this object are scaled
             *  2) The rotation and scale of the prefab were reset in blender
             */
            b_point = Vector3.Scale(trackPrefab.transform.localScale, b_point);
            b_point = trackPrefab.transform.rotation * b_point;
            b_point += trackPrefab.transform.position;
            bezierPoints[i] = b_point;
            
            i++;
        }

    }

    //debug method
    private void drawPath()
    {
        //https://en.wikipedia.org/wiki/B%C3%A9zier_curve
        
        for (int i = 0; i < bezierPoints.Length - 1; i ++)
        {
            DrawLine(bezierPoints[i], bezierPoints[i + 1], Color.red);
            //Debug.Log(bezierPoints[i]);
        }
    }

    void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.SetColors(color, color);
        lr.SetWidth(0.1f, 0.1f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }
}
