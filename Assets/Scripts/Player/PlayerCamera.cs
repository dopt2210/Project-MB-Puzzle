using UnityEngine;

[ExecuteInEditMode]
public class PlayerCamera : MonoBehaviour
{
    Camera cm;
    public float defaultFOV = 60;
    public float maxZoomFOV = 15;
    [Range(0, 1)]
    public float currentZoom;
    public float sensitivity = 1;


    void Awake()
    {
        // Spawn the cm on this gameObject and the defaultZoom.
        cm = GetComponent<Camera>();
        if (cm)
        {
            defaultFOV = cm.fieldOfView;
        }
    }

    void Update()
    {
        // Update the currentZoom and the cm's fieldOfView.
        currentZoom += Input.mouseScrollDelta.y * sensitivity * .05f;
        currentZoom = Mathf.Clamp01(currentZoom);
        cm.fieldOfView = Mathf.Lerp(defaultFOV, maxZoomFOV, currentZoom);
    }
}
