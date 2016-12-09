using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class OptionHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    private Text thisText;
    private Color initialColor;
    private int initialSize;
    private int hoverSize;

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
        thisText.color = hoverColor;
        thisText.fontSize = hoverSize;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        thisText.color = initialColor;
        thisText.fontSize = initialSize;
    }
}
