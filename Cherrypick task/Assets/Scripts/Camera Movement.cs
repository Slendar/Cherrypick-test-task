using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CameraMovement : MonoBehaviour
{
    private float maxZoomOut;
    private float minZoomIn;
    private float zoomSpeed = 1f;

    private float zoomValue = 0f;
    private Vector3 dragOrigin;
    private bool mousePressed = false;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private Slider zoomSlider;
    [SerializeField] private Spawner spawner;
    public GridCreator grid;

    private void Start()
    {
        int width = grid.gridArray.GetLength(0);
        int height = grid.gridArray.GetLength(1);
        int maxEdgeLenght = Mathf.Max(width, height);

        SetMaxMinZooms(maxEdgeLenght);

        zoomSlider.maxValue = maxZoomOut;
        zoomSlider.minValue = minZoomIn;
        zoomSpeed = minZoomIn;

        zoomValue = (maxZoomOut + minZoomIn) / 2;

        zoomSlider.value = zoomValue;
        mainCamera.orthographicSize = zoomValue;
        
        zoomSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    void Update()
    {
        PanCamera();
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel") * -1;

        if (scrollWheel != 0f)
        {
            zoomValue += scrollWheel * zoomSpeed;
            zoomValue = Mathf.Clamp(zoomValue, 1f, 100f);

            zoomValue = zoomValue <= maxZoomOut ? zoomValue : maxZoomOut;
            zoomValue = zoomValue >= minZoomIn ? zoomValue : minZoomIn;

            zoomSlider.value = zoomValue;
            mainCamera.orthographicSize = zoomValue;
        }
    }

    private void PanCamera()
    {
        if (Input.GetMouseButtonDown(0) && !spawner.isDragging)
        {
            if (!IsPointerOverUIObject())
            {
                mousePressed = true;
                dragOrigin = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        if (Input.GetMouseButton(0) && mousePressed)
        {
            Vector3 diffrence = dragOrigin - mainCamera.ScreenToWorldPoint(Input.mousePosition);

            mainCamera.transform.position += diffrence;
        }
        if (Input.GetMouseButtonUp(0))
        {
            mousePressed = false;
        }
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    private void SetMaxMinZooms(float longestEdge)
    {
        maxZoomOut = longestEdge / 2 + longestEdge * 0.05f;
        minZoomIn = longestEdge * 0.2f;
    }

    private void OnSliderValueChanged(float value)
    {
        zoomValue = value;

        mainCamera.orthographicSize = zoomValue;
    }
}
