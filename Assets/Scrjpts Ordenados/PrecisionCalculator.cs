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

    public void CalculatePrecision(
        List<GameObject> spheres,
        List<float> angles,
        List<float> arcLengths,
        List<float> speeds)
    {
        if (spheres.Count == 0)
        {
            resultText.text = "No spheres created.";
            return;
        }

        Vector3 lineDirection = (puntoB.position - puntoA.position).normalized;
        int inLineCount = 0;

        foreach (var sphere in spheres)
        {
            Vector3 sphereDirection = sphere.transform.position - puntoA.position;
            float distance = Vector3.Cross(lineDirection, sphereDirection).magnitude;
            if (distance <= tolerance) inLineCount++;
        }

        float precision = (float)inLineCount / spheres.Count * 100f;
        precisionText.text = $"Precision: {precision:F2}%";

        float angleScore = CalculateScore(angles, 80, 90);
        float arcScore = CalculateScore(arcLengths, 1.5f, 3f);
        float speedScore = CalculateScore(speeds, 3f, 6f);
        float totalScore = angleScore + arcScore + speedScore + (precision / 5f);

        bool approved = gameSettings.dificultad switch
        {
            "Dificil" => totalScore >= 90,
            "Normal" => totalScore >= 80,
            _ => totalScore >= 70
        };

        resultText.text = $"Score: {totalScore:F2}/100\n{(approved ? "APPROVED" : "FAILED")}";
    }

    private float CalculateScore(List<float> values, float min, float max)
    {
        if (values.Count == 0) return 0;
        float average = values.Average();
        float midpoint = (min + max) / 2;
        float score = 20 - (Mathf.Abs(average - midpoint) * 20 / (max - min));
        return Mathf.Clamp(score, 0, 20);
    }
}