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
    private GameObject _GrabbableCube;

    private ARPlaneManager _planeManager;
    private bool _isVisible = true;
    private int _numPlanesAddedOccurred=0;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("-> SceneController::Start()");

        _planeManager = GetComponent<ARPlaneManager>();

        if (_planeManager is null)
        {
            Debug.LogError("-> cant find ARPPLANEMANAGER");
        }

        _togglePlanesAccion.action.performed += OnTogglePlanesAction;
        _planeManager.planesChanged += OnPlanesChanged;
        _activateAction.action.performed += OnActivateAction;
    }
    private void OnActivateAction(InputAction.CallbackContext obj)
    {
        SpawnGrabbableCube();
    }
    private void SpawnGrabbableCube()
    {
        Debug.Log("-> SceneController :: SpawnGrabbableCube()");

        Vector3 SpawnPosition;

        foreach(var Plane in _planeManager.trackables)
        {
            if(Plane.classification == UnityEngine.XR.ARSubsystems.PlaneClassification.Table)
            {
                SpawnPosition = Plane.transform.position;
                SpawnPosition.y += 0.3f;
                Instantiate(_GrabbableCube, SpawnPosition, Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTogglePlanesAction(InputAction.CallbackContext obj)
    {
        _isVisible = !_isVisible;
        float fillAlpha = _isVisible ? 0.3f : 0f;
        float lineAlpha = _isVisible ? 1f : 0f;

        Debug.Log("-> OnTogglePlanesAction()-Trackable.count " + _planeManager.trackables.count);
       
        foreach(var plane in _planeManager.trackables)
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
            // Get the current start and end colors
            Color startColor = lineRenderer.startColor;
            Color endColor = lineRenderer.endColor;

            // Set the alpha component
            startColor.a = lineAlpha;
            endColor.a = lineAlpha;

            // Apply the new colors with updated alpha
            lineRenderer.startColor = startColor;
            lineRenderer.endColor = endColor;
        }
    }
    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        if (args.added.Count > 0)
        {
            _numPlanesAddedOccurred++;

            foreach (var plane in _planeManager.trackables)
            {
                PrintPlaneLabel(plane);
            }

            Debug.Log("-> Number of planes: " + _planeManager.trackables.count);
            Debug.Log("-> Num Planes Added Occurred: " + _numPlanesAddedOccurred);
        }
    }

    private void PrintPlaneLabel(ARPlane plane)
    {
        string label = plane.classification.ToString();
        string log = $"Plane ID: {plane.trackableId}, Label: {label}";
        Debug.Log(log);
    }
    private void OnDestroy()
    {
        Debug.Log("-> SceneController::OnDestroy()");
        _togglePlanesAccion.action.performed -= OnTogglePlanesAction;
        _planeManager.planesChanged -= OnPlanesChanged;
        _activateAction.action.performed -= OnActivateAction;
    }
}
