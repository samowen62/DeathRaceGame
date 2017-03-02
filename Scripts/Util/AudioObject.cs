using UnityEngine;
using UnityEngine.Assertions;

public class AudioObject : PausableBehaviour
{

    public float startTime;

    public int duplicates = 1;

    private AudioSource audio;

    private bool isPlaying;

    //TODO:add param for how many copies we want to make in the
    //event we need to clone the audio source to play twice or more at once

    // Use this for initialization
    protected override void _awake () {
        audio = GetComponent<AudioSource>();

        Assert.IsTrue(startTime >= 0);
        Assert.IsTrue(duplicates > 0);
    }

    // Update is called once per frame
    protected override void _update () {
	
	}

    public void Play()
    {
        audio.Play();
        audio.time = startTime;
    }

    protected override void onPause()
    {
        isPlaying = audio.isPlaying;
        if (isPlaying)
        {
            audio.Pause();
        }
    }

    protected override void onUnPause()
    {
        if (isPlaying)
        {
            audio.Play();
        }
    }
}
