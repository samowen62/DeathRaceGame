using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DAX_PrefCollideGen_V1_r2 : MonoBehaviour 
{
	public GameObject[] Items;
	public int[] RandForceIndices;
	public Vector3 minForce, maxForce;
	public string[] Descr;
	public Text OutText;
	public Text OutDescr;
	public int curIndex = 0;
	
	public bool RandomizeOnAwake = false;

	//public string groundMask = "GroundHit";
	//int gMask;

	bool lAction = false;

	void Awake()
	{
		//gMask = LayerMask.GetMask ( groundMask );
		
		if (RandomizeOnAwake)
		{	
			int j = Items.Length;
			List<GameObject> tmp = new List<GameObject>();	
			for (int i=0;i<Items.Length;i++)
			{
				tmp.Add( Items[i] );
			}
				
			for (int i = 0; i<Items.Length; i++) 
			{
				int rand = Random.Range(0,j);
				Items[i] = tmp[rand]; 
				tmp.RemoveAt(rand);
				j--;
			}
		}
	}

	void FixedUpdate () 
	{
		this.OutText.text = string.Format( "{0}/{1}", this.curIndex+1, this.Items.Length );

		if (!lAction) 
		{
			if (Input.GetMouseButton (0)) 
			{
				lAction = true;
				Turn ();
			}
		} 
		else 
		{
			if (!Input.GetMouseButton (0)) 
			{
				lAction = false;
			}
		}
	}

	void Update()
	{

	}

	public void Next()
	{
		this.curIndex += 1;
		if (this.curIndex >= this.Items.Length )
		{
			this.curIndex = 0;
		}
	}

	public void Prev()
	{
		this.curIndex -= 1;
		if (this.curIndex <0) { this.curIndex = this.Items.Length-1;};
	}

	public void  showPrefab( Vector3 Point)
	{
		GameObject tmpPref; 

		Vector3 vPoint = Point;
		vPoint.y += this.Items[ this.curIndex ].transform.position.y; 

		tmpPref = Instantiate( this.Items[ this.curIndex ], vPoint, Quaternion.identity) as GameObject;

		Rigidbody RB = tmpPref.GetComponent<Rigidbody> ();
		if (RB != null) 
		{
			RB.AddForce( 5.0f *Random.Range (this.minForce.x, this.maxForce.x), 5.0f * Random.Range (this.minForce.y, this.maxForce.y), 5.0f *Random.Range (this.minForce.z, this.maxForce.z), ForceMode.Impulse);
		}

//		this.OutDescr.text = this.Items [this.curIndex].ToString();
		/*if (this.OutDescr != null) 
		{
			this.OutDescr.text = this.Descr [this.curIndex];
		}*/
	}




	void Turn()
	{
		
		Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit GroundHit;
		if(Physics.Raycast (camRay, out GroundHit, 1000.0f, -1/*this.gMask*/))
		{
			showPrefab( GroundHit.point ); 
		}
	}

}
