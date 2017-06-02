using UnityEngine;
using System.Collections;

public class DAX_Flame_OnTimer_Destructor : MonoBehaviour 
{
	ParticleSystem[] ChildedPS;
	public float Timer = 5.0f;
	float eTime;

	// Use this for initialization
	void Start () 
	{
		this.ChildedPS = this.GetComponentsInChildren<ParticleSystem>();
		this.eTime = this.Timer;
	}
	

	// Update is called once per frame
	void Update () 
	{
		if (this.eTime > 0.0f) 
		{
			this.eTime -= Time.deltaTime;

			if (this.eTime <= 0.0f)
			{
				DAX_Start_Destruction_cBack();
			}
		}
	}

	public void DAX_Start_Destruction_cBack()
	{
		if (ChildedPS!=null)
		{
			for(int i=0; i<this.ChildedPS.Length; i++)
			{
				if (this.ChildedPS[i] != null)
				{
					this.ChildedPS[i].loop = false; 
				}
			}
			StartCoroutine("PSAliveCouroutine");
		}
	}

	IEnumerator PSAliveCouroutine ()
	{
		for(int i=0; i<this.ChildedPS.Length; i++)
		{
			if (this.ChildedPS[i] != null)
			{
				while ( this.ChildedPS[i].IsAlive() )
				{
					yield return new WaitForSeconds(0.5f);
				}
				GameObject.Destroy( this.ChildedPS[i].gameObject );
			}
		}
		
		GameObject.Destroy(this.gameObject);
	}
}
