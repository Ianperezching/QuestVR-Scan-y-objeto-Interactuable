using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARPlaneManager))]
public class SceneController : MonoBehaviour
{
    [SerializeField]
    private InputActionReference _togglePlanesAccion;

    [SerializeField]
    private InputActionReference _activateAction;

    [SerializeField]
    private GameObject _GrabbableSphere;  // Prefab de la esfera a instanciar

    [SerializeField]
    private Transform weldingGunTip;  // Transform de la punta de la pistola de soldadura

    private ARPlaneManager _planeManager;
    private bool _isVisible = true;

    void Start()
    {
        _planeManager = GetComponent<ARPlaneManager>();

        if (_planeManager is null)
        {
            Debug.LogError("-> No se encontró ARPlaneManager");
        }

        _togglePlanesAccion.action.performed += OnTogglePlanesAction;
        _activateAction.action.performed += OnActivateAction;
    }

    private void OnActivateAction(InputAction.CallbackContext obj)
    {
        SpawnWeldingSphere();
    }

    private void SpawnWeldingSphere()
    {
        if (_GrabbableSphere != null && weldingGunTip != null)
        {
            Vector3 spawnPosition = weldingGunTip.position;
            Instantiate(_GrabbableSphere, spawnPosition, Quaternion.identity);
        }
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

    private void OnDestroy()
    {
        _togglePlanesAccion.action.performed -= OnTogglePlanesAction;
        _activateAction.action.performed -= OnActivateAction;
    }
}
