using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    private new CinemachineCamera camera;
    private float defaultSize;
    private float currentSize;
    private float targetSize;
    private bool isZooming = false;

    void Awake()
    {
        // Initialize camera in Awake instead of Start
        InitializeCamera();
    }

    void Start()
    {
        // Double-check camera initialization
        if (camera == null)
        {
            InitializeCamera();
        }
    }

    private void InitializeCamera()
    {
        camera = GetComponent<CinemachineCamera>();
        if (camera == null)
        {
            Debug.LogError($"No CinemachineCamera found on {gameObject.name}!");
            return;
        }

        var lens = camera.Lens;
        defaultSize = lens.OrthographicSize;
        currentSize = defaultSize;
        targetSize = defaultSize;
        Debug.Log($"Camera initialized with size: {defaultSize}");
    }

    public void ZoomToSize(float size, float duration)
    {
        if (camera == null)
        {
            Debug.LogError("Cannot zoom - camera not initialized!");
            return;
        }

        if (!isZooming)
        {
            Debug.Log($"Zooming to size: {size}");
            targetSize = size;
            StartCoroutine(ZoomCoroutine(duration));
        }
    }

    public void ResetZoom(float duration)
    {
        if (!isZooming)
        {
            targetSize = defaultSize;
            StartCoroutine(ZoomCoroutine(duration));
        }
    }

    private IEnumerator ZoomCoroutine(float duration)
    {
        if (camera == null)
        {
            Debug.LogError("Camera reference is null in ZoomCoroutine!");
            yield break;
        }

        isZooming = true;
        float startSize = currentSize;
        float elapsed = 0;

        while (elapsed < duration)
        {
            if (camera == null)
            {
                Debug.LogError("Camera reference was lost during zoom!");
                break;
            }

            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t); // Smooth step interpolation
            
            currentSize = Mathf.Lerp(startSize, targetSize, t);
            var lens = camera.Lens;
            lens.OrthographicSize = currentSize;
            camera.Lens = lens;
            
            yield return null;
        }

        if (camera != null)
        {
            var finalLens = camera.Lens;
            finalLens.OrthographicSize = targetSize;
            camera.Lens = finalLens;
            currentSize = targetSize;
        }
        
        isZooming = false;
    }
}