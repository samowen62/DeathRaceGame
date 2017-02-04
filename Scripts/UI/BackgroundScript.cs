using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BackgroundScript : MonoBehaviour {

    public bool paused { get; set; }

    public Font menuFont;

    //instance variables 
    private Rect rect, guiBoxRect;
    private GUIStyle style;
    private Color guiBoxColor;

    void Awake()
    {
        if(menuFont == null)
        {
            Debug.LogError("Must attach a font to this background script");
        }

        guiBoxRect = new Rect(0, 0, 1000, 1000);
        guiBoxColor = new Color(0.9f, 0.9f, 0.9f);

        float w = 0.35f;
        float h = 0.2f;
        rect = new Rect(Screen.width * w, Screen.height * h, (Screen.width * (1 - w)) / 2, (Screen.height * (1 - h)) / 2);
        style = new GUIStyle();
        style.fontSize = 30;
        style.font = menuFont;
        style.normal.textColor = new Color32(0xD9, 0xD9, 0xD9, 0xFF);
        style.alignment = TextAnchor.MiddleCenter;
    }

    void OnGUI()
    {
        if (paused)
        {
            GUI.Box(guiBoxRect, "");
            GUI.color = guiBoxColor;
            GUI.Label(rect, "Paused Game", style);
        }

    }
}
