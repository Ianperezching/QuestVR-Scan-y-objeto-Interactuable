using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class PrecisionCalculator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform puntoA;
    [SerializeField] private Transform puntoB;
    [SerializeField] private float tolerance = 0.1f;
    [SerializeField] private GameSettings gameSettings;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text precisionText;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text angleText;
    [SerializeField] private TMP_Text arcLengthText;
    [SerializeField] private TMP_Text speedText;

    public void CalculatePrecision(List<GameObject> spheres, List<float> angles, List<float> arcLengths, List<float> speeds)
    {
        if (spheres.Count == 0)
        {
            resultText.text = "No spheres created.";
            ResetMetricsDisplay();
            return;
        }

        // Calcular precisión en línea
        Vector3 lineDirection = (puntoB.position - puntoA.position).normalized;
        int inLineCount = 0;

        foreach (var sphere in spheres)
        {
            if (sphere == null) continue;

            Vector3 sphereDirection = sphere.transform.position - puntoA.position;
            float distance = Vector3.Cross(lineDirection, sphereDirection).magnitude;
            if (distance <= tolerance) inLineCount++;
        }

        // Calcular promedios
        float precision = (float)inLineCount / spheres.Count * 100f;
        float avgAngle = angles.Count > 0 ? angles.Average() : 0f;
        float avgArcLength = arcLengths.Count > 0 ? arcLengths.Average() : 0f;
        float avgSpeed = speeds.Count > 0 ? speeds.Average() : 0f;

        // Actualizar UI
        UpdateMetricsDisplay(precision, avgAngle, avgArcLength, avgSpeed);

        // Calcular puntuación
        float angleScore = CalculateScore(angles, 80, 90);
        float arcScore = CalculateScore(arcLengths, 1.5f, 3f);
        float speedScore = CalculateScore(speeds, 0.4f, 1f);
        float totalScore = angleScore + arcScore + speedScore + (precision / 5f);

        // Evaluar aprobación
        bool approved = gameSettings.dificultad switch
        {
            "Dificil" => totalScore >= 90,
            "Normal" => totalScore >= 80,
            _ => totalScore >= 70
        };

        resultText.text = $"Score: {totalScore:F2}/100\n{(approved ? "APPROVED" : "FAILED")}";
    }

    private void UpdateMetricsDisplay(float precision, float angle, float arcLength, float speed)
    {
        precisionText.text = $"Precision: {precision:F2}%";
        angleText.text = $"Avg Angle: {angle:F2}°";
        arcLengthText.text = $"Arc Length: {arcLength:F2}m";
        speedText.text = $"Speed: {speed:F2}m/s";
    }

    private void ResetMetricsDisplay()
    {
        precisionText.text = "Precision: 0.00%";
        angleText.text = "Avg Angle: 0.00°";
        arcLengthText.text = "Arc Length: 0.00m";
        speedText.text = "Speed: 0.00m/s";
    }

    private float CalculateScore(List<float> values, float min, float max)
    {
        if (values.Count == 0) return 0;
        float average = values.Average();
        float midpoint = (min + max) / 2;
        float score = 20 - (Mathf.Abs(average - midpoint)) * 20 / (max - min);
        return Mathf.Clamp(score, 0, 20);
    } 
}