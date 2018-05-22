using UnityEngine;
using System.Collections;

public class HealingPlatform : MonoBehaviour
{

    public float Speed = 1;
    public Material ChosenMaterial; //In case the mesh has more than 1 material
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        if(ChosenMaterial != null)
        {
            ChosenMaterial.SetColor("_Color", HSBColor.ToColor(new HSBColor(Mathf.PingPong(Time.time * Speed, 1), 1, 1)));
        }
        else
        {
            rend.material.SetColor("_Color", HSBColor.ToColor(new HSBColor(Mathf.PingPong(Time.time * Speed, 1), 1, 1)));
        }
        
    }
}