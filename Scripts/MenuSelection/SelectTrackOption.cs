using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;

public class SelectTrackOption : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    private Text thisText;
    private Color initialColor;
    private Color disableColor = Color.gray;
    private int initialSize;
    private int hoverSize;

    private bool disabled = false;

    public Color hoverColor;

    void Start()
    {
        thisText = transform.Find("Text").GetComponent<Text>();
        initialColor = thisText.color;
        initialSize = thisText.fontSize;
        hoverSize = initialSize + 2;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (disabled) return;

        if (MouseEnter != null)
            MouseEnter.Invoke();
        //TODO:enlarge gif here

        thisText.color = hoverColor;
        thisText.fontSize = hoverSize;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (disabled) return;
        //TODO:retract gif here

        thisText.color = initialColor;
        thisText.fontSize = initialSize;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Not orking");
        if (Click != null)
            Click.Invoke();
    }

    public void disable()
    {
        disabled = true;
        thisText.color = disableColor;
        thisText.fontSize = initialSize;
    }

    public void enable()
    {
        if (disabled)
        {
            disabled = false;
            thisText.color = initialColor;
            thisText.fontSize = initialSize;
        }
    }

    public delegate void ClickHandler();
    public delegate void MouseEnterHanlder();
    public ClickHandler Click;
    public MouseEnterHanlder MouseEnter;
}

