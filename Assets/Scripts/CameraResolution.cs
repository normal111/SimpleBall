using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    private void Awake()
    {
        Application.targetFrameRate = 60;

        Camera camera = GetComponent<Camera>();
        Rect rect = camera.rect;
        float scaleHeight = ((float)Screen.width / Screen.height) / (9f / 16f);
        float scaleWidth = 1f / scaleHeight;
        if (scaleHeight < 1)
        {
            rect.height = scaleHeight;
            rect.y = (1f - scaleHeight) * 0.5f;
        }
        else
        {
            rect.width = scaleWidth;
            rect.x = (1f - scaleWidth) * 0.5f;
        }
        camera.rect = rect;
    }
}
