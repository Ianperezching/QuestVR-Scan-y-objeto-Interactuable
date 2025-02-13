using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField]
    private TMP_Text angleText;
    [SerializeField]
    private TMP_Text arcLengthText;
    [SerializeField]
    private TMP_Text speedText;
    [SerializeField]
    private TMP_Text trajectoryText;

    [Header("Configuración del juego")]
    [SerializeField]
    private GameSettings gameSettings;

    private ARPlaneManager _planeManager;
    private bool _isVisible = true;
    private bool _isSpawning = false;
    private Transform currentGunTip;
    private List<GameObject> spawnedSpheres = new List<GameObject>();
    private Vector3 lastPosition;
    private float elapsedTime;
    private List<float> speedMeasurements = new List<float>();
    private List<float> recordedAngles = new List<float>();
    private List<float> recordedArcLengths = new List<float>();

    void Start()
    {
        _planeManager = GetComponent<ARPlaneManager>();
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

        SetModeSettings();
        lastPosition = currentGunTip.position;
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

    private void StartSpawning(InputAction.CallbackContext obj)
    {
        if (!_isSpawning)
        {
            _isSpawning = true;
            recordedAngles.Clear();
            recordedArcLengths.Clear();
            speedMeasurements.Clear();
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
                GameObject sphere = Instantiate(_GrabbableSphere, currentGunTip.position, Quaternion.identity);
                spawnedSpheres.Add(sphere);
                RecordStatistics();
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

    private void SetModeSettings()
    {
        if (gameSettings.modo == "SMAW")
        {
            currentGunTip = weldingGunTip1;
            weldingGunModel1.SetActive(true);
            weldingGunModel2.SetActive(false);
        }
        else
        {
            currentGunTip = weldingGunTip2;
            weldingGunModel2.SetActive(true);
            weldingGunModel1.SetActive(false);
        }
    }

    private void CalculatePrecision()
    {
        if (spawnedSpheres.Count == 0)
        {
            resultText.text = "No hay esferas creadas.";
            return;
        }

        float averageAngle = recordedAngles.Count > 0 ? recordedAngles.Average() : 0;
        float averageArcLength = recordedArcLengths.Count > 0 ? recordedArcLengths.Average() : 0;
        float averageSpeed = speedMeasurements.Count > 0 ? speedMeasurements.Average() : 0;

        angleText.text = $"Ángulo Promedio: {averageAngle:F2}°";
        arcLengthText.text = $"Longitud de Arco Promedio: {averageArcLength:F2}m";
        speedText.text = $"Velocidad Promedio: {averageSpeed:F2} m/s";

        float score = Random.Range(60f, 100f);
        resultText.text = "Puntuación: " + score.ToString("F2");

        bool aprobado = (gameSettings.dificultad == "Dificil" && score >= 85) ||
                         (gameSettings.dificultad == "Normal" && score >= 70) ||
                         (gameSettings.dificultad == "Facil" && score >= 60);

        resultText.text += aprobado ? "\nAprobado" : "\nReprobado";
    }

    private void RecordStatistics()
    {
        if (currentGunTip == null) return;

        float arcAngle = Vector3.Angle(currentGunTip.forward, Vector3.up);
        recordedAngles.Add(arcAngle);

        float arcLength = Physics.Raycast(currentGunTip.position, -currentGunTip.up, out RaycastHit hit) ? hit.distance : 0f;
        recordedArcLengths.Add(arcLength);

        float distanceMoved = Vector3.Distance(lastPosition, currentGunTip.position);
        elapsedTime += Time.deltaTime;
        if (elapsedTime > 0.1f)
        {
            float speed = distanceMoved / elapsedTime;
            speedMeasurements.Add(speed);
            elapsedTime = 0;
        }
        lastPosition = currentGunTip.position;
    }
}
