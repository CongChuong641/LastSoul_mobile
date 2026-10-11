using UnityEngine;
using UnityEngine.EventSystems;

public class MobileAimPad : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private RectTransform padRect;

    private void Awake()
    {
        padRect = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        UpdateAimPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdateAimPosition(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

    private void UpdateAimPosition(PointerEventData eventData)
    {
        Vector2 screenPosition = eventData.position;

        if (padRect != null && !RectTransformUtility.RectangleContainsScreenPoint(padRect, screenPosition, eventData.pressEventCamera))
        {
            return;
        }

        GameInput.SetAimScreenPosition(screenPosition);
        GameInput.RequestFire();
    }
}