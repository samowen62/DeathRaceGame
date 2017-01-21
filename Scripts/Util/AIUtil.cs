using UnityEngine;
using System.Collections;

public static class AIUtil {

    public const float degToRad = Mathf.PI / 180f;

	public static float getHorizontal(Vector3 curr_pos, Vector3 curr_tangent, float current_speed, TrackPoint next)
    {
        return Vector3.Angle(curr_tangent, next.tangent) * current_speed * degToRad / (15f * (curr_pos - next.transform.position).magnitude);
    }
}
