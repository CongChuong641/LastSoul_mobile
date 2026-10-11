using UnityEngine;
using UnityEngine.EventSystems;

public class MobileJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform handle;
    [SerializeField] private float handleRange = 80f;

    private RectTransform background;
    private Vector2 inputVector;

    public void SetHandle(RectTransform joystickHandle)
    {
        handle = joystickHandle;
    }

    public void SetHandleRange(float range)
    {
        handleRange = Mathf.Max(1f, range);
    }

    private void Awake()
    {
        background = GetComponent<RectTransform>();
    }

    private void OnDisable()
    {
        inputVector = Vector2.zero;
        if (handle != null)
        {
            handle.anchoredPosition = Vector2.zero;
        }

        GameInput.ClearMoveInput();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(background, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            return;
        }

        localPoint = Vector2.ClampMagnitude(localPoint, handleRange);
        inputVector = localPoint / handleRange;

        if (handle != null)
        {
            handle.anchoredPosition = localPoint;
        }

        GameInput.SetMoveInput(inputVector);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;

        if (handle != null)
        {
            handle.anchoredPosition = Vector2.zero;
        }

        GameInput.ClearMoveInput();
    }
}