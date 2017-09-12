using UnityEngine;
using System.Xml;
using System.Linq;
using System.IO;
using System.Collections.Generic;

public class Track : MonoBehaviour {

    public CheckPoint startingCheckPoint;

    public TrackPoint initialTrackPoint;

    public float baseWidth;

    public bool isTrackReversed = true;

    public Vector3 starting_point;

    public TextAsset asset;

    private List<TrackPoint> points = new List<TrackPoint>();

    private GameObject trackPrefab;

    public bool creatingTrackPoints = false;

    public int totalTrackPoints
    {
        get
        {
            return points.Count;
        }
        set
        {

        }
    }

    // Use this for initialization
    void Awake () {
        if (creatingTrackPoints)
        {
            validateTrack();

            readTrackData();
        } else
        {
            points = transform.GetComponentsInChildren<TrackPoint>().ToList();
        }
	}

    public TrackPoint findClosestTrackPointTo(Vector3 target, TrackPoint.PathChoice pathChoice)
    {
        //TODO maybe avoid Linq if too much linking overhead
        return points
            .Where(point => point.pathChoice == pathChoice)
            .OrderByDescending(point => (point.transform.position - target).sqrMagnitude)
            .Last();
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
     */
    private void readTrackData()
    {

        XmlDocument doc = new XmlDocument();
        doc.Load(new MemoryStream(asset.bytes));

        System.Action<string, TrackPoint.PathChoice> addPointsFunction =
            (string parentNode, TrackPoint.PathChoice trackChoice) =>
        {
            var trackPointList = doc.GetElementsByTagName(parentNode);
            if (trackPointList.Count > 0)
            {
                generatePoints(trackPointList.Item(0).ChildNodes, trackChoice);
            }
        };

        addPointsFunction("TrackPoints", TrackPoint.PathChoice.PATH_A);
        addPointsFunction("TrackPoints-a", TrackPoint.PathChoice.PATH_A);
        addPointsFunction("TrackPoints-b", TrackPoint.PathChoice.PATH_B);
        addPointsFunction("TrackPoints-c", TrackPoint.PathChoice.PATH_C);
    }



    private void calculateTangents(TrackPoint[] trackPoints)
    {
        int len = trackPoints.Length;
        for (int i = 0; i < len; i++)
        {
            Vector3 tangent = (trackPoints[(i + 1) % len].transform.position - trackPoints[(i - 1 + len) % len].transform.position);
            trackPoints[i].GetComponent<SphereCollider>().radius = baseWidth / 2;// tangent.magnitude / 2;//TODO:This needs to extend the width of the track as well
            trackPoints[i].tangent = isTrackReversed ? -tangent.normalized : tangent.normalized;
            trackPoints[i].next = trackPoints[(i + 1) % len];
        }
    }

    private void generatePoints(XmlNodeList pointList, TrackPoint.PathChoice pathChoice)
    {
        if (pointList.Count == 0)
        {
            Debug.LogError("Need to have point data in xml file!");
        }

        //TODO: use bezier points to read in width. Currently we do nothing with them
        /*var bezierPointList = doc.GetElementsByTagName("BezierPoint");
        if (bezierPointList.Count == 0)
        {
            Debug.LogError("Need to have bezier point data in xml file!");
        }*/

        var trackPoints = new TrackPoint[pointList.Count];
        int i = 0;

        GameObject TrackPointParent = new GameObject();
        TrackPointParent.name = "TrackPointParent";
        TrackPointParent.transform.position = Vector3.zero;

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
            b_point = Quaternion.AngleAxis(180, Vector3.up) * b_point;

            /*
             * This will transform the bezier points into the real track's 
             * coordinates. It makes 2 assumptions:
             *  1) No parents of this object are scaled
             *  2) The rotation and scale of the prefab were reset in blender
             */
            b_point = Vector3.Scale(trackPrefab.transform.localScale, b_point);
            b_point = trackPrefab.transform.rotation * b_point;
            b_point += trackPrefab.transform.position;

            TrackPoint new_point = Instantiate(initialTrackPoint, b_point, Quaternion.identity, TrackPointParent.transform);

            new_point.tag = "TrackPoint";
            new_point.pathChoice = pathChoice;
            new_point.gameObject.AddComponent<SphereCollider>();
            new_point.gameObject.GetComponent<SphereCollider>().isTrigger = true;
            //new_point.width = System.Convert.ToSingle(point.ChildNodes.Item(3).InnerText);
            new_point.width = baseWidth;

            trackPoints[i] = new_point;
            i++;
        }

        //TODO read in function that will read in the entire array of widths and then correctly 
        //linearly interpolate through all the points[] to set the correct width multiplier for
        //each point. This should be multiplied by the scale of the track to get the proper width

        calculateTangents(trackPoints);

        points.AddRange(trackPoints.ToList());

        TrackPoint track_point = findClosestTrackPointTo(starting_point, pathChoice);
        int point_num = 1;

        while (track_point.num_in_seq == -1)
        {
            track_point.num_in_seq = point_num;
            track_point.name = "TrackPoint " + point_num;
            track_point = track_point.next;
            point_num++;
        }
    }
}
