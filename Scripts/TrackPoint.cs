using UnityEngine;

//TODO: refactor checkPoint class with this so there is a boolean
//whether this is a checkPoint. (Maybe every 6 of these is one?)
public class TrackPoint : MonoBehaviour {

    public Vector3 tangent { set; get; }
    public Vector3 normal { set; get; }
    public TrackPoint next { set; get; }
    public float width { set; get; }
}
