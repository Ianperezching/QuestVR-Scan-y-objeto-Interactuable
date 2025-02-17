using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using TMPro;

public class PrecisionCalculatora : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private Transform PuntoA;
    [SerializeField] private Transform PuntoB;
    [SerializeField] private float tolerance = 0.1f;
    [SerializeField] private GameSettings gameSettings;

    [Header("UI")]
    [SerializeField] private TMP_Text Presiciontext;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text angleText;
    [SerializeField] private TMP_Text arcLengthText;
    [SerializeField] private TMP_Text speedText;
    [SerializeField] private TMP_Text trajectoryText;

    private List<float> speedMeasurements = new List<float>();
    private List<float> recordedAngles = new List<float>();
    private List<float> recordedArcLengths = new List<float>();

    public void CalculatePrecision(List<GameObject> spawnedSpheres, List<float> angles, List<float> arcLengths, List<float> speeds)
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

}
