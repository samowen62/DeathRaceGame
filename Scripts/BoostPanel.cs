using UnityEngine;
using System.Collections;

public class BoostPanel : MonoBehaviour {

    public float movement_period = 1f;
    public float movement_amplitude = 3f;
    public float movement_spin_speed = 1f;

    //TODO: put layermasks in seperate enum class
    private int groundMask = 1 << 8;

    //TODO: SAME AS HOVER HEIGHT (this should be set in RacePlayer.cs and CheckPoint.cs as well so far)
    //Height of boost panels above the track
    private float trackHeight = 1.8f;

    private MeshRenderer innerCone;
    private MeshRenderer outerCone;
    private Vector3 starting_center;

    //constants relating to the boost animation
    //TODO: fix this and the Update() function for a paused game
    private float spin_animation_start_time = 0f;
    private float spin_animation_cumulative_time = 0f;
    private float spin_animation_time = 1.2f;
    private bool spin_animation_running = false;
    private Vector3 spin_animation_scale = 1.4f * Vector3.one;

    // Update is called once per frame
    void Update()
    {
        if (spin_animation_running)
        {
            float time_in_seq = Time.realtimeSinceStartup - spin_animation_start_time;//TODO: factor in pause here
            time_in_seq /= spin_animation_time;

            transform.localScale = Vector3.Lerp(spin_animation_scale, Vector3.one, time_in_seq);

            innerCone.transform.RotateAround(transform.position, transform.forward, 6 * movement_spin_speed * Time.deltaTime);
            outerCone.transform.RotateAround(transform.position, transform.forward, -6 * movement_spin_speed * Time.deltaTime);

            if (time_in_seq > spin_animation_time)
            {
                spin_animation_running = false;
                spin_animation_cumulative_time += Time.realtimeSinceStartup - spin_animation_start_time;
            }
        }
        else
        {
            float relativeHeight = movement_amplitude * Mathf.Sin((Time.realtimeSinceStartup - spin_animation_cumulative_time) * movement_period);

            transform.position = starting_center + transform.up * relativeHeight;

            innerCone.transform.RotateAround(transform.position, transform.forward, movement_spin_speed * Time.deltaTime);
            outerCone.transform.RotateAround(transform.position, transform.forward, -movement_spin_speed * Time.deltaTime);
        }
    }

    /**
     * This is called when a player hits a boost panel
     */
    public void boostAnimation()
    {
        Debug.Log("animation firing");
        spin_animation_running = true;
        spin_animation_start_time = Time.realtimeSinceStartup;
    }

    void Awake()
    {
        findCones();

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
     * MAKE SURE Y-AXIS OF PANEL IS MANUALLY ORIENTED RELATIVELY CLOSE TO DESIRED Y-AXIS
     */
    private void orientPanel()
    {
        RaycastHit downHit;

        if (Physics.Raycast(gameObject.transform.position, -gameObject.transform.up, out downHit, 30f, groundMask))
        {
            transform.rotation = Quaternion.LookRotation(transform.forward, downHit.normal);
            transform.position += transform.up * (trackHeight - downHit.distance);
        }
        else
        {
            Debug.LogError("Error: cannot find track to BoostPanel (" + this.name + "). Please orient this boost panel's y-axis up relative to the track and place the center above the track");
        }
    }

}
