using UnityEngine;

//TODO: refactor checkPoint class with this so there is a boolean
//whether this is a checkPoint. (Maybe every 6 of these is one?)

public class TrackPoint : MonoBehaviour {

    public Vector3 tangent;
    public TrackPoint next;
    public float width;
    public PathChoice pathChoice;

    public int num_in_seq;

    private void Awake()
    {
        num_in_seq = -1;
    }

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

    public enum PathChoice
    {
        PATH_A,
        PATH_B,
        PATH_C
    }

}
