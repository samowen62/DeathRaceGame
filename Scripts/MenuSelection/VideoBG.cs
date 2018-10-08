using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoBG : MonoBehaviour {

    void Start()
    {
        transform.Find("Video").GetComponent<VideoPlayer>().Play();
    }
}
