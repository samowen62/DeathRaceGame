using UnityEngine;
using System.Collections;

public class DAX_Flame_Auto_Destructor : MonoBehaviour 
{
	
	public GameObject[] ImmediateDestroyList;
	public GameObject[] AwaitDestroyList;
	public float AwaitDestroyTime = 5.0f;

	public bool StartOnAwake = true;
	
	ParticleSystem[] ChildedPS;
	
	// Use this for initialization
	void Start () 
	{
		this.ChildedPS = this.GetComponentsInChildren<ParticleSystem>();
		if ( StartOnAwake )
		{
			this.DAX_Start_Destruction_cBack();
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	
	public void DAX_Start_Destruction_cBack()
	{
		if (this.ImmediateDestroyList!=null)
		{
			for (int i=0;i<this.ImmediateDestroyList.Length;i++)
			{
				if (this.ImmediateDestroyList[i]!=null)
				{
					GameObject.Destroy( this.ImmediateDestroyList[i] );
				}
			}
		}
		
		if (ChildedPS!=null)
		{
			StartCoroutine("PSAliveCouroutine");
			
			if (this.ImmediateDestroyList!=null)
			{
				for(int i=0; i<this.AwaitDestroyList.Length; i++)
				{
					if (this.AwaitDestroyList[i] != null)
					{
						GameObject.Destroy(this.AwaitDestroyList[i], this.AwaitDestroyTime);
					}
				}
			}
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
