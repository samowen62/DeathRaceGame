using UnityEngine;

//TODO: make pool of these to be used by multiple racers
public class Explosion : PausableBehaviour
{
    private Animator animation;

    private MeshRenderer[] renderers;

    public Material material;

    public AudioObject sound;

    //TODO: verify
    private bool isPlaying;

    protected override void _awake () {
        animation = GetComponent<Animator>();
        renderers = GetComponentsInChildren<MeshRenderer>();
        isPlaying = false;

        foreach (var r in renderers)
        {
            r.material = material;
        }

        setInvisible();
    }
	
	public void Trigger (Vector3 pos, Quaternion rot, float seconds) {
        transform.position = pos;
        transform.rotation = rot;
        setVisible();
        animation.SetInteger("shouldExplode", 1);
        sound.Play();
        isPlaying = true;

        callAfterSeconds(seconds, () =>
        {
            isPlaying = false;
            setInvisible();
            animation.SetInteger("shouldExplode", -1);
        });
    }

    protected override void onPause()
    {
        //if (isPlaying)
            //animation.Stop();
    }

    protected override void onUnPause()
    {
        //if(isPlaying)
          //  animation.Play("Explosion");
    }

    private void setInvisible()
    {
        foreach(var r in renderers)
        {
            r.enabled = false;
        }
    }

    private void setVisible()
    {
        foreach (var r in renderers)
        {
            r.enabled = true;
        }
    }
}
