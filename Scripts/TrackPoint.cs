using System.Collections.Generic;
using UnityEngine;

public class TrackPoint : MonoBehaviour {

    [HideInInspector]
    public Vector3 tangent;

    [HideInInspector]
    public TrackPoint next;

    public int num_in_seq;

    public PathChoice pathChoice;

    public bool isCheckPoint;

    public List<TrackPoint> nextValidCheckPoints;

    //for checkpoints on only 1 path and to avoid turning backwards. checkpoints need a  list<int> to signify which num_in_seq's are of the next valid checkpoint
    //i.e. if 2 is on the branch 1->2,3 and 2->3

    public float width;

    /**
     * returns the length of the distance traversed in this track point
     * (i.e. how far into it we are) -radius to +radius of SphereCollider
     * 
     * Used for telling how far along a racer is for placement
     */
    public float distanceTraversed(Vector3 position)
    {
        //switch sign for Track.isTrackReversed
        return -Vector3.Dot((transform.position - position), tangent);
    }

    public bool isNextValidCheckPoint(TrackPoint trackPoint)
    {
        if (!isCheckPoint || pathChoice != trackPoint.pathChoice)
        {
            return false;
        }

        //return true if it is a valid next checkpoint or this checkpoint
        return trackPoint.num_in_seq == num_in_seq || nextValidCheckPoints.Contains(trackPoint);
    }

    public enum PathChoice
    {
        PATH_A,
        PATH_B,
        PATH_C
    }

}
