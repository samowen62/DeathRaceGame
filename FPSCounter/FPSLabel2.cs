using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(FPSCounter))]
public class FPSLabel2 : MonoBehaviour {

    public Text fpsLabel;

    FPSCounter fpsCounter;

    void Awake()
    {
        fpsCounter = GetComponent<FPSCounter>();
        fpsLabel.color = new Color(0.0f, 0.65f, 0.9f);
    }

    void Update()
    {
        fpsLabel.text = Mathf.Clamp(fpsCounter.FPS, 0, 99).ToString();
    }
}
