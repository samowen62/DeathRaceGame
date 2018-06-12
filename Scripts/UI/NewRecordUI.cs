using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewRecordUI : PausableBehaviour
{

    private Text centerText;
    private Image image;

    private bool _active = false;
    private float _speed = 0.15f;

    public void Activate()
    {
        _active = true;
        centerText.enabled = true;
        image.enabled = true;
    }

    protected override void _awake()
    {
        centerText = transform.Find("Text").GetComponent<Text>();
        image = GetComponent<Image>();
        centerText.enabled = false;
        image.enabled = false;
    }

    protected override void _update()
    {
        if(_active)
            centerText.color = HSBColor.ToColor(new HSBColor(Mathf.PingPong(Time.time * _speed, 1), 1, 1));
    }

    protected override void onPause()
    {
        centerText.enabled = false;
        image.enabled = false;
    }

    protected override void onUnPause()
    {
        if (_active)
        {
            centerText.enabled = true;
            image.enabled = true;
        }
    }
}
