using UnityEngine;
using UnityEngine.EventSystems;

public class SpawnButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public Spawner spawner;

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        StartCoroutine(spawner.SpawnItems());
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        StopAllCoroutines();
    }
}
