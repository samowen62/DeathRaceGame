using UnityEngine;
using System.Collections;

public class RacingUI : MonoBehaviour {

    public Font menuFont;

    //instance variables 
    private Rect position;
    private GUIStyle style;
    private Color guiBoxColor;

    void Awake()
    {
        if (menuFont == null)
        {
            Debug.LogError("Must attach a font to this background script");
        }

        transform.position = Camera.main.ViewportToWorldPoint(new Vector3(1f, 2f, 0));

        float w = 0.35f;
        float h = 0.2f;
        position = new Rect(Screen.width * w, Screen.height * h, 1000, 100);
        style = new GUIStyle();
        style.fontSize = 30;
        style.font = menuFont;
        style.normal.textColor = new Color32(0xD9, 0xD9, 0xD9, 0xFF);
        style.alignment = TextAnchor.MiddleCenter;
    }

    void OnGUI()
    {
         //GUI.color = guiBoxColor;
         //GUI.Label(position, "0 mph");
    }

}
