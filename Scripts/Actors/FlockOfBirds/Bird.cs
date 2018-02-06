using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : PausableBehaviour
{
    private SkinnedMeshRenderer meshRenderer;
    private Animator animator;

    void Awake () {
        meshRenderer = transform.Find("Bird").GetComponent<SkinnedMeshRenderer>();
        meshRenderer.enabled = false;

        animator = GetComponent<Animator>();
    }
	
	public void StartPlaying()
    {
        meshRenderer.enabled = true;
    }

    public void StopPlaying()
    {
        meshRenderer.enabled = false;
    }
}
