using UnityEngine;

[System.Serializable]
public class SpinScript : PausableBehaviour {

    [SerializeField]
    public float spinSpeed = 1f;

    [SerializeField]
    private int _choiceIndex = 0;
    [HideInInspector]
    public int ChoiceIndex
    {
        get
        {
            return _choiceIndex;
        }
        set
        {
            if (_choiceIndex == value) return;
            _choiceIndex = value;
        }
    }

    private Vector3 momentAxis { get; set; }

    protected override void _update()
    {
        transform.RotateAround(transform.position, momentAxis, 6 * spinSpeed * Time.deltaTime);
    }

    protected override void _awake()
    {
        var choices = new[] { transform.up, transform.right, transform.forward };
        momentAxis = choices[_choiceIndex];
    }
}

