using UnityEngine;
using System.Collections;

public class BannerScroll : MonoBehaviour {

    private Renderer banner;
    public float banner_speed = 0.6f;
    private float pause_start = 0f;

    /* This is for pausing the game */
    private float timePaused = 0f;
    private bool _behaviorBlocked;

    /*
     * TODO: for basic animations use .fbx files and find out how to progamatically pause them
     */
    public bool behaviorBlocked
    {
        get
        {
            return _behaviorBlocked;
        }
        set
        {
            if (value)
            {
                pause_start = Time.fixedTime;
            }
            else
            {
                timePaused += Time.fixedTime - pause_start;
            }
            _behaviorBlocked = value;
        }
    }

    void Start () {
        banner = GetComponent<MeshRenderer>();
	}
	
	void Update () {
        if (!_behaviorBlocked)
        {
            banner.material.mainTextureOffset = new Vector2(banner_speed * (Time.fixedTime - timePaused), 0);

        }
    }
}
