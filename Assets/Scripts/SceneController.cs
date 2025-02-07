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
    private Transform weldingGunTip1;
    [SerializeField]
    private Transform weldingGunTip2;
    [SerializeField]
    private GameObject weldingGunModel1;
    [SerializeField]
    private GameObject weldingGunModel2;

    [Header("Cambio de Pistola")]
    [SerializeField]
    private Button gun1Button;
    [SerializeField]
    private Button gun2Button;

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

    // UI para estadísticas
    [SerializeField]
    private TMP_Text angleText;
    [SerializeField]
    private TMP_Text arcLengthText;
    [SerializeField]
    private TMP_Text speedText;
    [SerializeField]
    private TMP_Text trajectoryText;

    private ARPlaneManager _planeManager;
    private bool _isVisible = true;
    private bool _isSpawning = false;
    private Transform currentGunTip;

    private List<GameObject> spawnedSpheres = new List<GameObject>();
    private Vector3 lastPosition;
    private float totalDistance;
    private float elapsedTime;
    private List<float> speedMeasurements = new List<float>();

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

        if (gun1Button != null)
        {
            gun1Button.onClick.AddListener(() => SwitchGun(weldingGunTip1, weldingGunModel1, weldingGunModel2));
        }

        if (gun2Button != null)
        {
            gun2Button.onClick.AddListener(() => SwitchGun(weldingGunTip2, weldingGunModel2, weldingGunModel1));
        }

        currentGunTip = weldingGunTip1; // Pistola por defecto
        weldingGunModel1.SetActive(true);
        weldingGunModel2.SetActive(false);

        lastPosition = currentGunTip.position;
    }

    void Update()
    {
        UpdateStatistics();
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
            if (_GrabbableSphere != null && currentGunTip != null)
            {
                Vector3 spawnPosition = currentGunTip.position;
                GameObject sphere = Instantiate(_GrabbableSphere, spawnPosition, Quaternion.identity);
                spawnedSpheres.Add(sphere);
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    private void SwitchGun(Transform newGunTip, GameObject newGunModel, GameObject oldGunModel)
    {
        currentGunTip = newGunTip;
        newGunModel.SetActive(true);
        oldGunModel.SetActive(false);
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

    private void UpdateStatistics()
    {
        if (currentGunTip == null) return;

        // Ángulo de Arco (Ángulo entre el electrodo y la superficie)
        Vector3 surfaceNormal = Vector3.up;
        float arcAngle = Vector3.Angle(currentGunTip.forward, surfaceNormal);
        angleText.text = $"Ángulo de Arco: {arcAngle:F2}°";

        // Longitud de Arco (Distancia de la punta de la pistola a la superficie)
        RaycastHit hit;
        float arcLength = 0f;
        if (Physics.Raycast(currentGunTip.position, -currentGunTip.up, out hit))
        {
            arcLength = hit.distance;
        }
        arcLengthText.text = $"Longitud de Arco: {arcLength:F2}m";

        // Velocidad de Recorrido (Rapidez con la que se mueve la pistola)
        float distanceMoved = Vector3.Distance(lastPosition, currentGunTip.position);
        elapsedTime += Time.deltaTime;
        if (elapsedTime > 0.1f)
        {
            float speed = distanceMoved / elapsedTime;
            speedMeasurements.Add(speed);
            if (speedMeasurements.Count > 10) speedMeasurements.RemoveAt(0);
            speedText.text = $"Velocidad: {speed:F2} m/s";
            elapsedTime = 0;
        }
        lastPosition = currentGunTip.position;

        // Trayectoria (Si el movimiento fue estable o errático)
        float speedVariance = Mathf.Abs(speedMeasurements.Count > 1 ? Mathf.Max(speedMeasurements.ToArray()) - Mathf.Min(speedMeasurements.ToArray()) : 0);
        trajectoryText.text = $"Trayectoria: {(speedVariance < 0.1f ? "Estable" : "Irregular")}";
    }
}
