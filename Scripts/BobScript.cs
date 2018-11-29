using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
    using UnityEditor;
#endif  

public class BobScript : PausableBehaviour {

    public float distance = 20f;
    public float speed = 4f;
    public float offset = 0f;

    private Vector3 initialPosition;

    protected override void _awake()
    {
        initialPosition = transform.position;
    }

    protected override void _update()
    {
        transform.position = initialPosition + new Vector3(0, distance * Mathf.Sin(speed * pauseInvariantTime + offset));
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(BobScript))]
public class EditorRaycastEditor : Editor
{
    // draws a line where this gameobject will bob
    void OnSceneGUI()
    {
        var t = target as BobScript;
        Handles.color = Color.cyan;
        Handles.DrawLine(t.transform.position + new Vector3(0, t.distance),
            t.transform.position + new Vector3(0, -t.distance));
    }
}
#endif