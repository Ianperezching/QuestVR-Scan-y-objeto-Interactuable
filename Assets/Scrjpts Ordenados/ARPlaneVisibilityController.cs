using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARPlaneManager))]
public class ARPlaneVisibilityController : MonoBehaviour
{
    [SerializeField] private InputActionReference togglePlanesAction;
    private ARPlaneManager planeManager;
    private bool isVisible = true;

    void Start()
    {
        planeManager = GetComponent<ARPlaneManager>();
        togglePlanesAction.action.performed += OnTogglePlanesAction;
    }

    private void OnTogglePlanesAction(InputAction.CallbackContext context)
    {
        isVisible = !isVisible;
        float fillAlpha = isVisible ? 0.3f : 0f;
        float lineAlpha = isVisible ? 1f : 0f;

        foreach (var plane in planeManager.trackables)
        {
            SetPlaneAlpha(plane, fillAlpha, lineAlpha);
        }
    }

    private void SetPlaneAlpha(ARPlane plane, float fillAlpha, float lineAlpha)
    {
        var meshRenderer = plane.GetComponentInChildren<MeshRenderer>();
        var lineRenderer = plane.GetComponentInChildren<LineRenderer>();

        if (meshRenderer != null)
        {
            Color color = meshRenderer.material.color;
            color.a = fillAlpha;
            meshRenderer.material.color = color;
        }

        if (lineRenderer != null)
        {
            Color startColor = lineRenderer.startColor;
            Color endColor = lineRenderer.endColor;

            startColor.a = lineAlpha;
            endColor.a = lineAlpha;

            lineRenderer.startColor = startColor;
            lineRenderer.endColor = endColor;
        }
    }

    void OnDestroy()
    {
        togglePlanesAction.action.performed -= OnTogglePlanesAction;
    }
}