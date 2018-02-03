using UnityEngine;

public class BoostPanel : PausableBehaviour
{

    public float movement_period = 1f;
    public float movement_amplitude = 3f;
    public float movement_spin_speed = 1f;

    [Tooltip("Which path choice (A/B/C) is this sticking to")]
    public TrackPoint.PathChoice pathChoice = TrackPoint.PathChoice.PATH_A;

    //Height of boost panels above the track
    private float trackHeight = 3f;
    private float trackFindHeight = 10f;

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

        findCones();
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
     */
    private void orientPanel()
    {
        Track track = FindObjectOfType(typeof(Track)) as Track;

        RaycastHit downHit;
        TrackPoint closestPosition = track.findClosestTrackPointTo(transform.position, pathChoice);

        var orientedCorrectly = Physics.Raycast(transform.position, -transform.forward, out downHit, trackFindHeight, AppConfig.groundMask)
        || Physics.Raycast(transform.position, -transform.up, out downHit, trackFindHeight, AppConfig.groundMask)
        || Physics.Raycast(transform.position, transform.forward, out downHit, trackFindHeight, AppConfig.groundMask)
        || Physics.Raycast(transform.position, transform.up, out downHit, trackFindHeight, AppConfig.groundMask);
        if (!orientedCorrectly)
        {
            Debug.LogError("Error: cannot find track to BoostPanel (" + this.name + "). Please orient this boost panel's x axis parrallel to the track");
        }

        transform.rotation = Quaternion.LookRotation(downHit.normal, closestPosition.tangent);
        transform.position = trackHeight * downHit.normal + downHit.point;
    }

}
