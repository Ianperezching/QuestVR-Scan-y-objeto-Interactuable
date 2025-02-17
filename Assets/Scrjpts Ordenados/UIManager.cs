using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Botones")]
    [SerializeField] private Button calculateButton;
    [SerializeField] private Button gun1Button;
    [SerializeField] private Button gun2Button;

    [Header("Textos")]
    [SerializeField] private TMP_Text angleText;
    [SerializeField] private TMP_Text arcLengthText;
    // ... otros textos ...

    public void SetupUI(
        PrecisionCalculator precisionCalculator,
        WeldingGunController gunController,
        SphereSpawner spawner
    )
    {
        calculateButton.onClick.AddListener(() =>
            precisionCalculator.CalculatePrecision
        );
        // Configurar otros botones y eventos
    }
}
