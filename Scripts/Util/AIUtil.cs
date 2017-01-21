using UnityEngine;
using System.Collections;

public static class AIUtil {

    public const float degToRad = Mathf.PI / 180f;
    public const float speedFactor = 20f; 

	public static float getHorizontal(Vector3 curr_pos, Vector3 curr_tangent, float current_speed, TrackPoint next)
    {
        //Debug.Log((curr_pos - next.transform.position).magnitude) don't want this. Goes awkwardly from ~22 down to 1;
        return Vector3.Angle(curr_tangent, next.tangent) * current_speed * degToRad / speedFactor;
    }
}
