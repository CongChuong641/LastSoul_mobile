using UnityEngine;
using UnityEngine.EventSystems;

public class MobileActionButton : MonoBehaviour, IPointerDownHandler
{
    public enum ActionType
    {
        Fire,
        Reload,
        Pause
    }

    [SerializeField] private ActionType actionType;

    public void SetActionType(ActionType type)
    {
        actionType = type;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        switch (actionType)
        {
            case ActionType.Fire:
                GameInput.RequestFire();
                break;
            case ActionType.Reload:
                GameInput.RequestReload();
                break;
            case ActionType.Pause:
                GameInput.RequestPause();
                break;
        }
    }
}