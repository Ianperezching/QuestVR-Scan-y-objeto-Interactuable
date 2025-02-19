using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button calculateButton;
    [SerializeField] private Button gun1Button;
    [SerializeField] private Button gun2Button;

    [Header("Text Displays")]
    [SerializeField] private TMP_Text modeDisplay;
    [SerializeField] private TMP_Text difficultyDisplay;

    [Header("Dependencies")]
    [SerializeField] private WeldingStatsRecorder statsRecorder;
    [SerializeField] private SphereSpawner spawner;

    public void Initialize(
        WeldingGunController gunController,
        PrecisionCalculator precisionCalculator,
        SphereSpawner sphereSpawner
    )
    {
        spawner = sphereSpawner;

        calculateButton.onClick.AddListener(() =>
            precisionCalculator.CalculatePrecision(
                spawner.SpawnedSpheres,
                statsRecorder.RecordedAngles,
                statsRecorder.RecordedArcLengths,
                statsRecorder.SpeedMeasurements
            )
        );

        gun1Button.onClick.AddListener(() => gunController.SwitchGun(true));
        gun2Button.onClick.AddListener(() => gunController.SwitchGun(false));

    }
}