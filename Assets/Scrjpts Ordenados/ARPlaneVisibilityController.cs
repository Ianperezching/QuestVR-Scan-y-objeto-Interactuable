using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ARPlaneManager))]
public class ARPlaneVisibilityController : MonoBehaviour
{
    [SerializeField] private InputActionReference _togglePlanesAccion;
    private ARPlaneManager _planeManager;
    private bool _isVisible = true;

    void Start()
    { 
       _planeManager = GetComponent<ARPlaneManager>();
        _togglePlanesAccion.action.performed += OnTogglePlanesAction;
    }

    private void OnTogglePlanesAction(InputAction.CallbackContext obj)
    {
        _isVisible = !_isVisible;
        float fillAlpha = _isVisible ? 0.3f : 0f;
        float lineAlpha = _isVisible ? 1f : 0f;

        foreach (var plane in _planeManager.trackables)
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
}
