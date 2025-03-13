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

    [Header("Scale Settings")]
    [SerializeField] private float worldScale = 0.3f;      
    [SerializeField] private float displayMultiplier = 100f;

    public void CalculatePrecision(List<GameObject> spheres, List<float> angles, List<float> arcLengths, List<float> speeds)
    {
        if (spheres.Count == 0)
        {
            resultText.text = "No spheres created.";
            ResetMetricsDisplay();
            return;
        }

        // 1. Calcular precisión en la línea de soldadura
        Vector3 lineDirection = (puntoB.position - puntoA.position).normalized;
        int inLineCount = 0;

        foreach (var sphere in spheres)
        {
            if (sphere == null) continue;

            Vector3 spherePos = sphere.transform.position;
            float distanceToLine = Vector3.Cross(lineDirection, spherePos - puntoA.position).magnitude;
            if (distanceToLine <= tolerance) inLineCount++;
        }

        // 2. Calcular métricas base
        float precision = (float)inLineCount / spheres.Count * 100f;
        float avgAngle = angles.Count > 0 ? angles.Average() : 0f;

        // 3. Aplicar escalado y conversión a centímetros
        float scaledArcLength = arcLengths.Count > 0 ?
            arcLengths.Average() * worldScale * displayMultiplier : 0f;

        float scaledSpeed = speeds.Count > 0 ?
            speeds.Average() * worldScale * displayMultiplier : 0f;

        // 4. Actualizar UI
        UpdateMetricsDisplay(precision, avgAngle, scaledArcLength, scaledSpeed);

        // 5. Calcular puntuación con valores sin escalar para UI
        float angleScore = CalculateScore(angles, 80, 90);
        float arcScore = CalculateScore(arcLengths, 0.1f / worldScale, 1f / worldScale);
        float speedScore = CalculateScore(speeds, 3f / worldScale, 6f / worldScale);
        float totalScore = angleScore + arcScore + speedScore + (precision / 4f);

        // 6. Determinar resultado final
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
        precisionText.text = $"Precisión: {precision:F2}%";
        angleText.text = $"Ángulo: {angle:F2}°";
        arcLengthText.text = $"Longitud: {arcLength:F2} cm";
        speedText.text = $"Velocidad: {speed:F2} cm/s";
    }

    private void ResetMetricsDisplay()
    {
        precisionText.text = "Precisión: 0.00%";
        angleText.text = "Ángulo: 0.00°";
        arcLengthText.text = "Longitud: 0.00 cm";
        speedText.text = "Velocidad: 0.00 cm/s";
    }

    private float CalculateScore(List<float> values, float min, float max)
    {
        if (values.Count == 0) return 0;

        float average = values.Average();
        float distanceFromIdeal = Mathf.Abs(average - (min + max) / 2);
        float score = 20 - (distanceFromIdeal * 20 / (max - min));

        return Mathf.Clamp(score, 0, 20);
    }
}