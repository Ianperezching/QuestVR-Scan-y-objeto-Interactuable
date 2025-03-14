using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PistolaSphereCastMerge : MonoBehaviour
{
    public float sphereRadius = 1.0f;
    public float maxDistance = 5.0f;
    public LayerMask layerMask;
    private List<GameObject> detectedObjects = new List<GameObject>();
    public Transform pivot;
    bool Press;
    public ParticleSystem Spark;

    [SerializeField] private InputActionReference triggerAction;
    Vector3 normales = Vector3.zero;
    public bool IsGizmo = false;
    float FrameRate = 0;
    public float Rate;
    public ObjectPoolManager _ObjectPoolManager;

    private NewPart currentPart;
    public Text voltageText;
    public Text speedText;
    public Text timeText;
    public Text resultText;
    public Image panelImage;
    public Sprite defaultImage;
    public List<NewPartInfo> partInfos;

    [System.Serializable]
    public class NewPartInfo
    {
        public string partName;
        public Sprite partImage;
    }

    public float voltage = 22.0f;
    public float wireSpeed = 385f;
    public string weldingResult = "65% regular";

    [Header("Parámetros de Rendimiento")]
    public float velocidadActual;
    public float anguloActual;
    public float distanciaActual;
    public float estabilidadActual;
    private Vector3 posicionAnterior;
    private Queue<Vector3> posicionesRecientes = new Queue<Vector3>();
    private const int muestrasEstabilidad = 10;

    // Estadísticas de soldadura
    private float totalWeldingTime = 0f;
    private float correctSpheresTime = 0f;
    private float totalSpeed = 0f;
    private float totalAngle = 0f;
    private float totalDistance = 0f;
    private float totalStability = 0f;
    private int sampleCount = 0;

    void Start()
    {
        posicionAnterior = pivot.position;
        posicionesRecientes.Enqueue(pivot.position);
        ResetStatistics();
    }

    void FixedUpdate()
    {
        Press = (triggerAction.action.ReadValue<float>() > 0.2f);

        if (Press)
        {
            normales = Vector3.zero;
            RaycastHit[] hits;
            Vector3 direction = pivot.forward;

            hits = Physics.SphereCastAll(pivot.position, sphereRadius, direction, maxDistance, layerMask);
            detectedObjects.Clear();

            foreach (RaycastHit hit in hits)
            {
                if (!detectedObjects.Contains(hit.collider.gameObject))
                {
                    detectedObjects.Add(hit.collider.gameObject);
                }
                normales += hit.normal;
            }

            if (detectedObjects.Count == 2)
            {
                MergeObjects(detectedObjects[0], detectedObjects[1]);
                correctSpheresTime += Time.fixedDeltaTime;
            }

            totalWeldingTime += Time.fixedDeltaTime;

            if (detectedObjects.Count > 0)
            {
                RaycastHit hit = hits[0];
                CalcularParametrosAdicionales(hit);
                ActualizarUI();

                Quaternion rotation = Quaternion.LookRotation(normales, Vector3.up);
                Spark.transform.position = hit.point;
                Spark.transform.rotation = rotation;

                if (FrameRate > Rate)
                {
                    _ObjectPoolManager?.GetObject(hit.point, Quaternion.identity, detectedObjects[0].transform);
                    FrameRate = 0;
                }
                FrameRate += Time.deltaTime;

                if (!Spark.isPlaying)
                {
                    Spark.Play();
                }
            }
        }
        else
        {
            if (Spark.isPlaying)
            {
                Spark.Stop();
            }
        }
    }

    void CalcularParametrosAdicionales(RaycastHit hit)
    {
        // Cálculo de velocidad
        Vector3 desplazamiento = pivot.position - posicionAnterior;
        velocidadActual = desplazamiento.magnitude / Time.fixedDeltaTime;
        posicionAnterior = pivot.position;

        // Cálculo de ángulo
        Vector3 direccionPistola = pivot.forward;
        Vector3 normalSuperficie = hit.normal;
        anguloActual = Vector3.Angle(direccionPistola, normalSuperficie);

        // Cálculo de distancia
        distanciaActual = Vector3.Distance(pivot.position, hit.point);

        // Cálculo de estabilidad
        posicionesRecientes.Enqueue(pivot.position);
        if (posicionesRecientes.Count > muestrasEstabilidad)
            posicionesRecientes.Dequeue();

        estabilidadActual = CalcularVariacionTrayectoria();

        // Acumular datos para estadísticas
        totalSpeed += velocidadActual;
        totalAngle += anguloActual;
        totalDistance += distanciaActual;
        totalStability += estabilidadActual;
        sampleCount++;
    }

    float CalcularVariacionTrayectoria()
    {
        Vector3 sumaDirecciones = Vector3.zero;
        Vector3[] posiciones = posicionesRecientes.ToArray();

        if (posiciones.Length < 2) return 0f;

        for (int i = 1; i < posiciones.Length; i++)
        {
            sumaDirecciones += (posiciones[i] - posiciones[i - 1]).normalized;
        }

        Vector3 direccionPromedio = sumaDirecciones / (posiciones.Length - 1);
        float variacionTotal = 0f;

        for (int i = 1; i < posiciones.Length; i++)
        {
            Vector3 direccionActual = (posiciones[i] - posiciones[i - 1]).normalized;
            variacionTotal += Vector3.Angle(direccionActual, direccionPromedio);
        }

        return variacionTotal / (posiciones.Length - 1);
    }

    void ActualizarUI()
    {
        speedText.text = $"Velocidad: {velocidadActual:F2} m/s";
        voltageText.text = $"Ángulo: {anguloActual:F2}°";
        timeText.text = $"Distancia: {distanciaActual:F2} m";
        resultText.text = $"Estabilidad: {estabilidadActual:F2}°";
    }

    public void CalculateResults()
    {
        if (sampleCount == 0 || totalWeldingTime == 0)
        {
            resultText.text = "No hay datos de soldadura";
            return;
        }

        float avgSpeed = totalSpeed / sampleCount;
        float avgAngle = totalAngle / sampleCount;
        float avgDistance = totalDistance / sampleCount;
        float avgStability = totalStability / sampleCount;
        float precision = (correctSpheresTime / totalWeldingTime) * 100f;

        float score = CalculateScore(precision, avgSpeed, avgAngle, avgDistance, avgStability);
        bool aprobado = score >= 70f;

        resultText.text = $"PRECISIÓN: {precision:F1}%\n" +
                        $"VELOCIDAD: {avgSpeed:F2} m/s\n" +
                        $"ÁNGULO: {avgAngle:F2}°\n" +
                        $"DISTANCIA: {avgDistance:F2} m\n" +
                        $"ESTABILIDAD: {avgStability:F2}°\n\n" +
                        $"PUNTUACIÓN FINAL: {score:F0}/100\n" +
                        $"RESULTADO: {(aprobado ? "APROBADO" : "REPROBADO")}";
    }

    private float CalculateScore(float precision, float speed, float angle, float distance, float stability)
    {
        float precisionScore = precision;
        float speedScore = Mathf.Clamp(100f - Mathf.Abs(speed - 0.5f) * 100f, 0f, 100f);
        float angleScore = Mathf.Clamp(100f - Mathf.Abs(angle - 90f), 0f, 100f);
        float distanceScore = Mathf.Clamp(100f - distance * 50f, 0f, 100f);
        float stabilityScore = Mathf.Clamp(100f - stability * 10f, 0f, 100f);

        return Mathf.Clamp(
            (precisionScore * 0.5f) +
            (speedScore * 0.2f) +
            (angleScore * 0.2f) +
            (distanceScore * 0.1f) +
            (stabilityScore * 0.1f),
            0f, 100f
        );
    }

    public void ResetStatistics()
    {
        totalWeldingTime = 0f;
        correctSpheresTime = 0f;
        totalSpeed = 0f;
        totalAngle = 0f;
        totalDistance = 0f;
        totalStability = 0f;
        sampleCount = 0;
        resultText.text = "Estadísticas reiniciadas";
    }

    void MergeObjects(GameObject obj1, GameObject obj2)
    {
        NewPart part1 = obj1.GetComponent<NewPart>();
        NewPart part2 = obj2.GetComponent<NewPart>();
        if (part1 == null || part2 == null) return;

        if (part1.weight > part2.weight)
        {
            part1.AbsorbPiece(part2);
        }
        else
        {
            part2.AbsorbPiece(part1);
        }
    }

    private void OnDrawGizmos()
    {
        if (!IsGizmo) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(pivot.position, pivot.position + pivot.forward * maxDistance);
        Gizmos.DrawWireSphere(pivot.position + pivot.forward * maxDistance, sphereRadius);

        normales = Vector3.zero;
        RaycastHit[] hits = Physics.SphereCastAll(pivot.position, sphereRadius, pivot.forward, maxDistance, layerMask);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Metal"))
            {
                normales += hit.normal;
            }
        }

        if (hits.Length > 0)
        {
            Gizmos.DrawLine(hits[0].point, hits[0].point + normales * 12);
        }
    }

    public bool IsWelding() => Press;
    public float GetVoltage() => voltage;
    public float GetWireSpeed() => wireSpeed;
    public string GetWeldingResult() => weldingResult;
}