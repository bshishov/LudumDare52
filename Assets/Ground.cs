using UnityEngine;
using UnityEngine.EventSystems;


public class Ground : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    public Vector3 LastPointerWorldPosition { get; private set; }

    public void OnPointerMove(PointerEventData eventData)
    {
        LastPointerWorldPosition = eventData.pointerCurrentRaycast.worldPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        LastPointerWorldPosition = eventData.pointerCurrentRaycast.worldPosition;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LastPointerWorldPosition = eventData.pointerCurrentRaycast.worldPosition;
    }
}
