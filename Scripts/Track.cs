using UnityEngine;
using System.Collections;
using System.Xml;

//TODO: use this class to store AI data
public class Track : MonoBehaviour {

    public CheckPoint startingCheckPoint;

    public TrackPoint initialTrackPoint;

    public string trackName;

    private TrackPoint[] points;

    //prefab of track
    private GameObject trackPrefab;

    private const string TRACK_DATA_ROOT = "/Prefabs/Tracks/";
    private const string TRACK_DATA_FILE_NAME = "/TrackData.xml";

    // Use this for initialization
    void Awake () {
        validateTrack();

        readTrackData();
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

        points = new TrackPoint[pointList.Count + 1];
        int i = 0;

        Vector3 trackScale = trackPrefab.transform.localScale;
        GameObject TrackPointParent = new GameObject();
        TrackPointParent.name = "TrackPointParent";
        TrackPointParent.transform.position = Vector3.zero;

        //TODO: put in xml file a node if the tangent direction is opposite to 
        //that of where the car should be pointed
        bool isTrackReversed = true;

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

            TrackPoint new_point = (TrackPoint) Instantiate(initialTrackPoint, b_point, Quaternion.identity, TrackPointParent.transform);
            new_point.name = "TrackPoint " + i;
            new_point.tag = "TrackPoint";
            new_point.gameObject.AddComponent<SphereCollider>();
            new_point.gameObject.GetComponent<SphereCollider>().isTrigger = true;

            points[i] = new_point;
            i++;
        }

        points[points.Length - 1] = points[0];

        calculateTangents(isTrackReversed);

        //drawPath();

        //drawTangents();
    }

    private void calculateTangents(bool _isTrackReversed)
    {
        int len = points.Length;
        for (int i = 0; i < len; i++)
        {
            Vector3 tangent = (points[(i + 1) % len].transform.position - points[(i - 1 + len) % len].transform.position);
            points[i].GetComponent<SphereCollider>().radius = tangent.magnitude / 2;
            points[i].tangent = _isTrackReversed ? -tangent.normalized : tangent.normalized;
            points[i].next = points[(i + 1) % len];
        }
    }

    /*
     * TODO: elaborate more on AI plan to determine if we even need normals to be saved
     * if we do:
     *  1) Raycast from point a guess at the normal direction 
     *      b = (point + tangent) X (point + nextPoint)
     *      n_guess = b X tangent
     *  2) cast up and down
     *  3) whichever hits use 
     *      mesh = (hit.collider as MeshCollider).sharedMesh
     *      triangles = mesh.triangles
     *      verticies = mesh.verticies
     *      Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
     *      Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
     *      Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];
     *  4) use the points to find the track normal (wonder if we can)
     * 
     */
    /*
    private void calculateNormals()
    {
        RaycastHit hit;

        foreach(var point in points)
        {
            if (Physics.Raycast(point.point,))
            {

            } else if ()
            {

            }
            else
            {
                Debug.LogError("Could not find a track normal for point " + point.point + " !!");
            }
        }     
    }*/

    private void drawPath()
    {
        for (int i = 0; i < points.Length - 1; i++)
        {
            DrawLine(points[i].transform.position, points[i + 1].transform.position, Color.red);
        }
    }

    private void drawTangents()
    {
        foreach(var point in points)
        {
            DrawLine(point.transform.position, point.transform.position + point.tangent, Color.blue);
        }
    }

    private void DrawLine(Vector3 start, Vector3 end, Color color)
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
