using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class MobileHUDBuilder
{
    private static readonly Color ButtonColor = new Color(1f, 1f, 1f, 0.85f);
    private static readonly Color JoystickColor = new Color(1f, 1f, 1f, 0.18f);
    private static readonly Color HandleColor = new Color(1f, 1f, 1f, 0.75f);
    private static CanvasGroup canvasGroup;

    public static void Build(GameManager gameManager)
    {
        if (GameObject.Find("MobileHUDCanvas") != null)
        {
            return;
        }

        EnsureEventSystem();

        GameObject canvasObject = new GameObject("MobileHUDCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        canvasObject.AddComponent<CanvasScaler>();
        CanvasScaler canvasScaler = canvasObject.GetComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
        canvasScaler.matchWidthOrHeight = 0.5f;
        canvasObject.AddComponent<GraphicRaycaster>();
        canvasGroup = canvasObject.AddComponent<CanvasGroup>();

        GameObject root = new GameObject("MobileHUDRoot", typeof(RectTransform));
        root.transform.SetParent(canvasObject.transform, false);
        RectTransform rootRect = root.GetComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;
        root.AddComponent<MobileSafeArea>();

        Sprite uiSprite = CreateSolidSprite(false);
        Sprite knobSprite = CreateSolidSprite(true);
        Font uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        CreateAimPad(root.transform, uiSprite);
        CreateLeftJoystick(root.transform, knobSprite);
        CreateActionControls(root.transform, uiSprite, uiFont, gameManager);
        Debug.Log("Mobile HUD preview created. Open the Game tab while Play Mode is active.");
    }

    public static void SetInteractable(bool interactable)
    {
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = interactable;
            canvasGroup.interactable = interactable;
        }
    }

    private static void EnsureEventSystem()
    {
        if (EventSystem.current != null)
        {
            return;
        }

        GameObject eventSystemObject = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        Object.DontDestroyOnLoad(eventSystemObject);
    }

    private static void CreateAimPad(Transform parent, Sprite uiSprite)
    {
        GameObject aimPad = CreateImageObject("AimPad", parent, uiSprite, new Color(1f, 1f, 1f, 0.01f));
        RectTransform aimPadRect = aimPad.GetComponent<RectTransform>();
        aimPadRect.anchorMin = Vector2.zero;
        aimPadRect.anchorMax = Vector2.one;
        aimPadRect.offsetMin = Vector2.zero;
        aimPadRect.offsetMax = Vector2.zero;
        aimPad.AddComponent<MobileAimPad>();
    }

    private static void CreateLeftJoystick(Transform parent, Sprite joystickSprite)
    {
        GameObject background = CreateImageObject("JoystickBackground", parent, joystickSprite, JoystickColor);
        RectTransform backgroundRect = background.GetComponent<RectTransform>();
        backgroundRect.anchorMin = new Vector2(0f, 0f);
        backgroundRect.anchorMax = new Vector2(0f, 0f);
        backgroundRect.pivot = new Vector2(0.5f, 0.5f);
        backgroundRect.anchoredPosition = new Vector2(250f, 250f);
        backgroundRect.sizeDelta = new Vector2(420f, 420f);

        GameObject handle = CreateImageObject("JoystickHandle", background.transform, joystickSprite, HandleColor);
        RectTransform handleRect = handle.GetComponent<RectTransform>();
        handleRect.anchorMin = new Vector2(0.5f, 0.5f);
        handleRect.anchorMax = new Vector2(0.5f, 0.5f);
        handleRect.pivot = new Vector2(0.5f, 0.5f);
        handleRect.anchoredPosition = Vector2.zero;
        handleRect.sizeDelta = new Vector2(190f, 190f);

        MobileJoystick joystick = background.AddComponent<MobileJoystick>();
        joystick.SetHandle(handleRect);
        joystick.SetHandleRange(175f);
    }

    private static void CreateActionControls(Transform parent, Sprite uiSprite, Font uiFont, GameManager gameManager)
    {
        Sprite circleSprite = CreateSolidSprite(true);
        CreateActionButton(parent, circleSprite, uiFont, "ReloadButton", "R", new Vector2(-170f, 160f), new Vector2(120f, 120f), MobileActionButton.ActionType.Reload);
        CreatePauseButton(parent, uiSprite, uiFont, gameManager);
    }

    private static void CreatePauseButton(Transform parent, Sprite uiSprite, Font uiFont, GameManager gameManager)
    {
        GameObject pauseButton = CreateActionButton(parent, uiSprite, uiFont, "PauseButton", "II", new Vector2(-40f, -40f), new Vector2(90f, 90f), MobileActionButton.ActionType.Pause);
        RectTransform pauseRect = pauseButton.GetComponent<RectTransform>();
        pauseRect.anchorMin = new Vector2(1f, 1f);
        pauseRect.anchorMax = new Vector2(1f, 1f);
        pauseRect.pivot = new Vector2(1f, 1f);
        pauseRect.anchoredPosition = new Vector2(-40f, -40f);
        Button button = pauseButton.GetComponent<Button>();
        button.onClick.AddListener(gameManager.PauseGameMenu);
    }

    private static GameObject CreateActionButton(Transform parent, Sprite uiSprite, Font uiFont, string name, string label, Vector2 anchoredPosition, Vector2 size, MobileActionButton.ActionType actionType)
    {
        GameObject buttonObject = CreateButtonObject(name, parent, uiSprite, ButtonColor, label, uiFont);
        RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(1f, 0f);
        rectTransform.anchorMax = new Vector2(1f, 0f);
        rectTransform.pivot = new Vector2(1f, 0f);
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = size;

        MobileActionButton actionButton = buttonObject.AddComponent<MobileActionButton>();
        actionButton.SetActionType(actionType);
        return buttonObject;
    }

    private static GameObject CreateButtonObject(string name, Transform parent, Sprite uiSprite, Color color, string label, Font uiFont)
    {
        GameObject buttonObject = CreateImageObject(name, parent, uiSprite, color);
        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = buttonObject.GetComponent<Image>();

        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(buttonObject.transform, false);

        Text text = textObject.AddComponent<Text>();
        text.font = uiFont;
        text.text = label;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = new Color(0.15f, 0.15f, 0.15f, 1f);
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = 18;
        text.resizeTextMaxSize = 36;

        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        return buttonObject;
    }

    private static GameObject CreatePanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Color color)
    {
        GameObject panel = CreateImageObject(name, parent, CreateSolidSprite(false), color);
        RectTransform rectTransform = panel.GetComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        return panel;
    }

    private static Sprite CreateSolidSprite(bool circle)
    {
        const int textureSize = 128;
        Texture2D texture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[textureSize * textureSize];
        Vector2 center = new Vector2((textureSize - 1) * 0.5f, (textureSize - 1) * 0.5f);
        float radius = textureSize * 0.5f - 1f;

        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                bool insideCircle = !circle || Vector2.Distance(new Vector2(x, y), center) <= radius;
                pixels[y * textureSize + x] = insideCircle ? Color.white : Color.clear;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        texture.name = circle ? "MobileHUDCircleTexture" : "MobileHUDSquareTexture";
        return Sprite.Create(texture, new Rect(0f, 0f, textureSize, textureSize), new Vector2(0.5f, 0.5f));
    }

    private static GameObject CreateImageObject(string name, Transform parent, Sprite sprite, Color color)
    {
        GameObject imageObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        imageObject.transform.SetParent(parent, false);
        Image image = imageObject.GetComponent<Image>();
        image.sprite = sprite;
        image.type = Image.Type.Simple;
        image.color = color;
        return imageObject;
    }
}