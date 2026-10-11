using UnityEngine;

public static class GameInput
{
    private static Vector2 moveInputOverride;
    private static bool hasMoveInputOverride;

    private static Vector2 aimScreenPositionOverride;
    private static bool hasAimScreenPositionOverride;

    private static bool fireRequested;
    private static bool reloadRequested;
    private static bool pauseRequested;

    public static Vector2 GetMoveInput()
    {
        if (Application.isMobilePlatform)
        {
            return moveInputOverride;
        }

        if (hasMoveInputOverride)
        {
            return moveInputOverride;
        }

        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    public static void SetMoveInput(Vector2 moveInput)
    {
        moveInputOverride = moveInput;
        hasMoveInputOverride = true;
    }

    public static void ClearMoveInput()
    {
        moveInputOverride = Vector2.zero;
        hasMoveInputOverride = false;
    }

    public static Vector2 GetAimScreenPosition()
    {
        if (hasAimScreenPositionOverride)
        {
            return aimScreenPositionOverride;
        }

        return Input.mousePosition;
    }

    public static void SetAimScreenPosition(Vector2 screenPosition)
    {
        aimScreenPositionOverride = screenPosition;
        hasAimScreenPositionOverride = true;
    }

    public static void ClearAimScreenPosition()
    {
        hasAimScreenPositionOverride = false;
    }

    public static void RequestFire()
    {
        fireRequested = true;
    }

    public static bool ConsumeFirePressed()
    {
        if (fireRequested)
        {
            fireRequested = false;
            return true;
        }

        if (Application.isMobilePlatform)
        {
            return false;
        }

        return Input.GetMouseButtonDown(0);
    }

    public static void RequestReload()
    {
        reloadRequested = true;
    }

    public static bool ConsumeReloadPressed()
    {
        if (reloadRequested)
        {
            reloadRequested = false;
            return true;
        }

        if (Application.isMobilePlatform)
        {
            return false;
        }

        return Input.GetMouseButtonDown(1);
    }

    public static void RequestPause()
    {
        pauseRequested = true;
    }

    public static bool ConsumePausePressed()
    {
        if (pauseRequested)
        {
            pauseRequested = false;
            return true;
        }

        if (Application.isMobilePlatform)
        {
            return false;
        }

        return Input.GetKeyDown(KeyCode.Escape);
    }
}