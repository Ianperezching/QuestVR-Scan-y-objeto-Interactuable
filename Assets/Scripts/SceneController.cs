using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(ARPlaneManager))]
public class SceneController : MonoBehaviour
{
    [Header("Configuración de la pistola de soldadura")]
    [SerializeField]
    private InputActionReference _togglePlanesAccion;
    [SerializeField]
    private InputActionReference _activateAction;
    [SerializeField]
    private GameObject _GrabbableSphere;  
    [SerializeField]
    private Transform weldingGunTip;      

    [Header("Configuración de la línea objetivo")]
    [SerializeField]
    private Transform PuntoA; 
    [SerializeField]
    private Transform PuntoB; 
    [SerializeField]
    private float tolerance = 0.1f; 

    [Header("UI")]
    [SerializeField]
    private Button calculateButton;  
    [SerializeField]
    private TMP_Text resultText;         

    private ARPlaneManager _planeManager;
    private bool _isVisible = true;
    private bool _isSpawning = false;

    private List<GameObject> spawnedSpheres = new List<GameObject>(); 

    void Start()
    {
        _planeManager = GetComponent<ARPlaneManager>();

        if (_planeManager == null)
        {
            Debug.LogError("-> No se encontró ARPlaneManager");
        }

        _togglePlanesAccion.action.performed += OnTogglePlanesAction;
        _activateAction.action.performed += StartSpawning;
        _activateAction.action.canceled += StopSpawning;

        if (calculateButton != null)
        {
            calculateButton.onClick.AddListener(CalculatePrecision);
        }
    }

    private void StartSpawning(InputAction.CallbackContext obj)
    {
        if (!_isSpawning)
        {
            _isSpawning = true;
            StartCoroutine(SpawnWeldingSpheres());
        }
    }

    private void StopSpawning(InputAction.CallbackContext obj)
    {
        _isSpawning = false;
    }

    private IEnumerator SpawnWeldingSpheres()
    {
        while (_isSpawning)
        {
            if (_GrabbableSphere != null && weldingGunTip != null)
            {
                Vector3 spawnPosition = weldingGunTip.position;
                GameObject sphere = Instantiate(_GrabbableSphere, spawnPosition, Quaternion.identity);
                spawnedSpheres.Add(sphere); // Añadir esfera a la lista
            }
            yield return new WaitForSeconds(0.3f);
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

    private void CalculatePrecision()
    {
        if (spawnedSpheres.Count == 0)
        {
            resultText.text = "No hay esferas creadas.";
            return;
        }

        Vector3 lineDirection = (PuntoB.position - PuntoA.position).normalized;
        int alignedCount = 0;

        foreach (GameObject sphere in spawnedSpheres)
        {
            if (sphere == null) continue;

            Vector3 sphereToLineStart = sphere.transform.position - PuntoA.position;
            float distanceToLine = Vector3.Cross(lineDirection, sphereToLineStart).magnitude / lineDirection.magnitude;

            if (distanceToLine <= tolerance)
            {
                alignedCount++;
            }
        }

        float precisionPercentage = (float)alignedCount / spawnedSpheres.Count * 100f;
        resultText.text = $"Precisión: {precisionPercentage:F2}%";
    }

    private void OnDestroy()
    {
        _togglePlanesAccion.action.performed -= OnTogglePlanesAction;
        _activateAction.action.performed -= StartSpawning;
        _activateAction.action.canceled -= StopSpawning;

        if (calculateButton != null)
        {
            calculateButton.onClick.RemoveListener(CalculatePrecision);
        }
    }
}

