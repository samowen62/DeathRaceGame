using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class SelectTrackOption : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    private Text text;
    private VideoPlayer gif;
    private Material planeMaterial;

    private Color initialColor;
    private Color greyedColor = new Color(0.25f, 0.25f, 0.25f);

    private Vector3 initialScale;
    private Vector3 largerScale = new Vector3(1.2f, 1.2f, 1.2f);

    private int initialSize;
    private int hoverSize;

    private float timeLastInteraction = 0f;

    void Start()
    {
        text = GetComponent<Text>();
        text.text = AppConfig.getTrackDisplayName(name);
        initialScale = transform.localScale;
        planeMaterial = transform.Find("Plane").GetComponent<Renderer>().material;
        planeMaterial.color = greyedColor;
        planeMaterial.SetColor("_EmissionColor", greyedColor);

        gif = transform.Find("Video").GetComponent<VideoPlayer>();
        gif.Prepare();
        gif.prepareCompleted += e => {
            e.Play();
            StartCoroutine(pauseWhenLoaded());
        };

        initialColor = text.color;
        initialSize = text.fontSize;
        hoverSize = initialSize + 2;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // to avoid flicker
        if (Math.Abs(Time.fixedTime - timeLastInteraction) < 0.1)
            return;
        timeLastInteraction = Time.fixedTime;

        if (MouseEnter != null)
            MouseEnter.Invoke();
        
        planeMaterial.SetColor("_EmissionColor", Color.white);

        gif.Play();
        text.fontSize = hoverSize;
        transform.localScale = largerScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        planeMaterial.color = greyedColor;
        planeMaterial.SetColor("_EmissionColor", greyedColor);

        gif.Pause();
        text.fontSize = initialSize;
        transform.localScale = initialScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Click != null)
            Click.Invoke();
    }

    private IEnumerator pauseWhenLoaded()
    {
        yield return new WaitUntil(() => gif.frame > 4);
        gif.Pause();
    }

    public delegate void ClickHandler();
    public delegate void MouseEnterHanlder();
    public ClickHandler Click;
    public MouseEnterHanlder MouseEnter;
}

