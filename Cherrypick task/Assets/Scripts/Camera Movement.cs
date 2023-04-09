using UnityEngine;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour
{
    [HideInInspector] public float maxZoomOut;
    private float maxZoomIn = 2;
    public Slider slider;

    public void SetMaxZoomOut(float maxZoomOut)
    {
        this.maxZoomOut = maxZoomOut;
    }
}
