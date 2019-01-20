using UnityEngine;
using System.Collections;

public static class AIUtil {

    
    private const float speedFactor = 20f;
    private const float halfWidthToComfortableWidth = 0.59f / 2f; //2f is for half width 

	public static float getHorizontal(Vector3 curr_pos, Vector3 curr_tangent, float current_speed, TrackPoint next)
    {
        return Vector3.Angle(curr_tangent, next.tangent) * current_speed * AppConfig.degToRad / speedFactor;
    }

    //-1 if should turn left 1 if right. 0 if neither
    public static int checkIfNearEdge(Vector3 curr_pos, Vector3 curr_normal, TrackPoint curr_point)
    {
        // TODO: add flag to TrackPoint to see if this point curves a lot. If so scale halfWidthToComfortableWidth back to keep closer towards center
        float distance_from_center = Vector3.Dot(curr_pos - curr_point.transform.position, Vector3.Cross(curr_normal, curr_point.tangent));
        float comfortableDistance = curr_point.width * halfWidthToComfortableWidth;
        if (curr_point.usesAISmoothing)
        {
            comfortableDistance *= 0.6f;
        }

        if (Mathf.Abs(distance_from_center) >= comfortableDistance)
        {
            return distance_from_center > 0 ? -1 : 1;
        }

        return 0;
    }
}
