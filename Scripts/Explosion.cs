using UnityEngine;

//TODO: make pool of these to be used by multiple racers
public class Explosion : PausableBehaviour
{
    private Animator animation;

    private MeshRenderer[] renderers;

    public Material material;

    public AudioObject sound;

    protected override void _awake () {
        animation = GetComponent<Animator>();
        renderers = GetComponentsInChildren<MeshRenderer>();
        
        foreach(var r in renderers)
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

        callAfterSeconds(seconds, () =>
        {
            setInvisible();
            animation.SetInteger("shouldExplode", -1);
        });
    }

    //TODO: figure out how to pause these!!
    //I think since the objects don't exist in the editor this funciton isn't called
    protected override void onPause()
    {
        Debug.Log("paused explosion");
        animation.Stop();
    }

    protected override void onUnPause()
    {
        animation.Play("Explosion");
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
