using UnityEngine;

public class Explosion : PausableBehaviour
{
    private Animator animation;

    private MeshRenderer[] renderers;

    public Material material;

    public AudioObject sound;

    protected override void _awake () {
        animation = GetComponent<Animator>();
        renderers = GetComponentsInChildren<MeshRenderer>();

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

        callAfterSeconds(seconds, () =>
        {
            setInvisible();
            animation.SetInteger("shouldExplode", -1);
        });
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
