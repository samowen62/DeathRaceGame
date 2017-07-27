using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class OptionHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
        thisText = GetComponent<Text>();
        initialColor = thisText.color;
        initialSize = thisText.fontSize;
        hoverSize = initialSize + 2;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (disabled) return;

        thisText.color = hoverColor;
        thisText.fontSize = hoverSize;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (disabled) return;

        thisText.color = initialColor;
        thisText.fontSize = initialSize;
    }

    public void disable()
    {
        disabled = true;
        thisText.color = disableColor;
        thisText.fontSize = initialSize;
    }

    public void enable()
    {
        if (disabled) {
            disabled = false;
            thisText.color = initialColor;
            thisText.fontSize = initialSize;
        }
    }
}
