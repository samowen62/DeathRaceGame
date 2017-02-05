using UnityEngine;
using System.Collections;

public class BannerScroll : PausableBehaviour
{

    private Renderer banner;

    public float banner_speed = 0.6f;

    protected override void _awake () {
        banner = GetComponent<MeshRenderer>();
    }

    protected override void _update () {
        if (!_behaviorBlocked)
        {
            banner.material.mainTextureOffset = new Vector2(banner_speed * pauseInvariantTime, 0);
        }
    }
}
