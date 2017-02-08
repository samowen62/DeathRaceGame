using UnityEngine;
using System.Collections;

public class BoostPanel : PausableBehaviour
{

    public float movement_period = 1f;
    public float movement_amplitude = 3f;
    public float movement_spin_speed = 1f;

    //Height of boost panels above the track
    private float trackHeight = 3f;

    private MeshRenderer innerCone;
    private MeshRenderer outerCone;
    private Vector3 starting_center;

    //constants relating to the boost animation
    private float spin_animation_start_time = 0f;
    private float spin_animation_cumulative_time = 0f;
    private float spin_animation_time = 1.2f;
    private bool spin_animation_running = false;
    private Vector3 spin_animation_scale = 1.4f * Vector3.one;

    // Update is called once per frame
    protected override void _update()
    {
        //TODO: wait for trackpoints to be updated a better concurrent way
        //Extend an AfterTrackLoaded class
        if (innerCone == null || outerCone == null)
            return;

        if (spin_animation_running)
        {
            float time_in_seq = pauseInvariantTime - spin_animation_start_time;
            time_in_seq /= spin_animation_time;

            transform.localScale = Vector3.Lerp(spin_animation_scale, Vector3.one, time_in_seq);

            innerCone.transform.RotateAround(transform.position, transform.up, 6 * movement_spin_speed * Time.deltaTime);
            outerCone.transform.RotateAround(transform.position, transform.up, -6 * movement_spin_speed * Time.deltaTime);

            if (time_in_seq > spin_animation_time)
            {
                spin_animation_running = false;
                spin_animation_cumulative_time += pauseInvariantTime - spin_animation_start_time;
            }
        }
        else
        {
            float relativeHeight = movement_amplitude * Mathf.Sin((pauseInvariantTime - spin_animation_cumulative_time) * movement_period);

            transform.position = starting_center + transform.forward * relativeHeight;

            innerCone.transform.RotateAround(transform.position, transform.up, movement_spin_speed * Time.deltaTime);
            outerCone.transform.RotateAround(transform.position, transform.up, -movement_spin_speed * Time.deltaTime);
        }
    }

    /**
     * This is called when a player hits a boost panel
     */
    public void boostAnimation()
    {
        spin_animation_running = true;
        spin_animation_start_time = pauseInvariantTime;
    }

    protected override void _awake()
    {
        orientPanel();
    }

    /**
     * sets the innerCone and outerCone instance prefabs
     */
    private void findCones () {
        starting_center = transform.position;

        var meshes = GetComponentsInChildren<MeshRenderer>();

        if (meshes.Length == 0)
            Debug.LogError("No inner/outer cone prefabs for gameObject " + name);

        foreach(var mesh in meshes)
        {
            switch (mesh.name)
            {
                case "InnerCone":
                    innerCone = mesh;
                    break;
                case "OuterCone":
                    outerCone = mesh;
                    break;
                default:
                    Debug.LogWarning("No prefab with name " + mesh.name + " is supported!");
                    break;
            }
        }
    }
	
    /**
     * Sets the panel to be level with the track
     * 
     * MAKE SURE Z-AXIS OF PANEL IS MANUALLY ORIENTED RELATIVELY CLOSE TO DESIRED Y-AXIS
     */
    private void orientPanel()
    {
        StartCoroutine(setOrientiationWhenTrackLoaded());       
    }

    IEnumerator setOrientiationWhenTrackLoaded()
    {
        Track track = FindObjectOfType(typeof(Track)) as Track;
        while (!track.loaded)
        {
            yield return null;
        }

        RaycastHit downHit;
        TrackPoint closestPosition = track.findClosestTrackPointTo(transform.position);

        if (Physics.Raycast(transform.position, -transform.forward, out downHit, 30f, AppConfig.groundMask))
        {
            transform.rotation = Quaternion.LookRotation(downHit.normal , closestPosition.tangent);
            transform.position -= transform.forward * (trackHeight - downHit.distance);
        }
        else
        {
            Debug.LogError("Error: cannot find track to BoostPanel (" + this.name + "). Please orient this boost panel's z-axis up relative to the track and place the center above the track");
        }

        findCones();

        yield return 0;
    }

}
