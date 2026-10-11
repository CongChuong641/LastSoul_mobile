using UnityEngine;

public class MobileCameraFraming : MonoBehaviour
{
    [SerializeField] private float referenceAspect = 16f / 9f;
    private float referenceOrthographicSize;

    private void Awake()
    {
        Camera cameraComponent = GetComponent<Camera>();
        if (cameraComponent == null || !cameraComponent.orthographic)
        {
            enabled = false;
            return;
        }

        referenceOrthographicSize = cameraComponent.orthographicSize;
        ApplyFraming(cameraComponent);
    }

    private void OnEnable()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
    }

    private void Update()
    {
        Camera cameraComponent = GetComponent<Camera>();
        if (cameraComponent != null)
        {
            ApplyFraming(cameraComponent);
        }
    }

    private void ApplyFraming(Camera cameraComponent)
    {
        if (Screen.height <= 0)
        {
            return;
        }

        float currentAspect = (float)Screen.width / Screen.height;
        cameraComponent.orthographicSize = referenceOrthographicSize * Mathf.Max(1f, referenceAspect / currentAspect);
    }
}