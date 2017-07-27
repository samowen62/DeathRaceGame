using UnityEngine.UI;
using UnityEngine;

public class RacerOption : MonoBehaviour {

    private MeshRenderer ship;

    private Image outline;

    private Color outlineColor;

    private float rotationSpeed = 2f;
    private float outlineFlashSpeed = 4f;
    private float scaleMultiplier = 1.6f;

    public bool selectedShip = false;

    void Awake () {
        Transform shipTransform = transform.Find("Ship");
        ship = shipTransform.GetComponent<MeshRenderer>();

        outline = transform.Find("Outline").GetComponent<Image>();
        outlineColor = outline.color;
        setOutline(0);
    }
	
	// Update is called once per frame
	void Update () {
        ship.transform.localRotation *= Quaternion.AngleAxis(rotationSpeed, Vector3.forward);
        if (selectedShip)
        {
            setOutline(0.3f * Mathf.Sin(outlineFlashSpeed * Time.fixedTime) + 0.7f);
        }
    }

    public void select()
    {
        if (!selectedShip)
        {
            selectedShip = true;
            ship.transform.localScale *= scaleMultiplier;
        }
    }

    public void deselect()
    {
        if (selectedShip)
        {
            setOutline(0);
            selectedShip = false;
            ship.transform.localScale /= scaleMultiplier;
        }
    }

    private void setOutline(float alpha)
    {
        alpha = Mathf.Clamp01(alpha);
        outlineColor.a = alpha;
        outline.color = outlineColor;
    }
}
