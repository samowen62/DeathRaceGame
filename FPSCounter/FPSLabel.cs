using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(FPSCounter))]
public class FPSDisplay : MonoBehaviour
{

    public Text fpsLabel;

    /*FPSCounter fpsCounter;

    void Awake()
    {
        fpsCounter = GetComponent<FPSCounter>();
    }

    void Update()
    {
        fpsLabel.text = fpsCounter.FPS.ToString();
    }*/
}
