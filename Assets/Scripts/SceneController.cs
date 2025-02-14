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
    private TMP_Text Presiciontext;
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

        // Obtener promedios
        float averageAngle = recordedAngles.Count > 0 ? recordedAngles.Average() : 0;
        float averageArcLength = recordedArcLengths.Count > 0 ? recordedArcLengths.Average() : 0;
        float averageSpeed = speedMeasurements.Count > 0 ? speedMeasurements.Average() : 0;

        angleText.text = $"Ángulo Promedio: {averageAngle:F2}°";
        arcLengthText.text = $"Longitud de Arco Promedio: {averageArcLength:F2}m";
        speedText.text = $"Velocidad Promedio: {averageSpeed:F2} m/s";

        // Evaluación de precisión en línea
        Vector3 lineDirection = (PuntoB.position - PuntoA.position).normalized;
        float totalSpheres = spawnedSpheres.Count;
        int inLineSpheres = 0;

        foreach (var sphere in spawnedSpheres)
        {
            if (sphere == null) continue;

            Vector3 sphereDirection = sphere.transform.position - PuntoA.position;
            float perpendicularDistance = Vector3.Cross(lineDirection, sphereDirection).magnitude / lineDirection.magnitude;

            if (perpendicularDistance <= tolerance)
            {
                inLineSpheres++;
            }
        }

        float precision = (float)inLineSpheres / totalSpheres * 100f;
        Presiciontext.text = $"Precisión: {precision:F2}%";

        // **Calculo de la puntuación basada en distancia al rango ideal**
        float scoreAngle = CalculateScore(averageAngle, 80, 90);
        float scoreArcAngle = CalculateScore(averageArcLength, 10, 20);
        float scoreArcLength = CalculateScore(averageArcLength, 1.5f, 3f);
        float scoreSpeed = CalculateScore(averageSpeed, 3f, 6f);

        // Precisión también contribuye a la puntuación total (20 puntos si está perfecta)
        float scorePrecision = (precision / 100f) * 20f;

        // **Puntuación total sobre 100**
        float totalScore = scoreAngle + scoreArcAngle + scoreArcLength + scoreSpeed + scorePrecision;

        // Determinar si aprueba según la dificultad
        bool aprobado = (gameSettings.dificultad == "Dificil" && totalScore >= 90) ||
                        (gameSettings.dificultad == "Normal" && totalScore >= 80) ||
                        (gameSettings.dificultad == "Facil" && totalScore >= 70);

        resultText.text = $"Puntuación: {totalScore:F2}/100";
        resultText.text += aprobado ? "\nAprobado" : "\nReprobado";
    }

    // Método para calcular la puntuación basada en qué tan cerca está del rango ideal
    private float CalculateScore(float value, float min, float max)
    {
        if (value >= min && value <= max)
        {
            return 20f; // Dentro del rango ideal, puntaje perfecto
        }
        else
        {
            // Penalización proporcional (si está fuera del rango)
            float distance = Mathf.Min(Mathf.Abs(value - min), Mathf.Abs(value - max));
            float maxPenalty = 20f; // Si está muy lejos, puede perder hasta los 20 puntos
            return Mathf.Max(0, 20f - (distance * maxPenalty / (max - min)));
        }
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
