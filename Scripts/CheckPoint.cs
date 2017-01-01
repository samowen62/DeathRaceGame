using UnityEngine;
using System.Collections;

public class CheckPoint : MonoBehaviour {

    public Vector3 trackPoint { get; set; }
    public Vector3 trackNormal { get; set; }
    public Vector3 trackForward { get; set; }
    public float yaw { get; set; }

    void Awake () {
        RaycastHit downHit;

        if (Physics.Raycast(gameObject.transform.position, -gameObject.transform.up, out downHit, GetComponent<SphereCollider>().radius, AppConfig.groundMask))
        {
            trackNormal = downHit.normal;
            trackPoint = downHit.point + (AppConfig.hoverHeight * downHit.normal);//this 1.8f needs to be the hover height as well
            trackForward = gameObject.transform.forward;
            yaw = (Quaternion.FromToRotation(transform.up, downHit.normal) * transform.rotation).eulerAngles.y;
        } else
        {
            Debug.LogError("Error: cannot find track to checkpoint (" + gameObject.name + "). Please orient this checkpoint's y-axis up relative to the track and place the center above the track");
        }
    }



}
