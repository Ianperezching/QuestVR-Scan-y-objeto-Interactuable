using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject mainUICanvas; // Canvas principal de la UI

    [Header("Controles Meta Quest")]
    [SerializeField] private InputActionReference toggleUIAction; // Botón del mando (ej: Menú)

    [Header("Buttons")]
    [SerializeField] private Button calculateButton;
    

    [Header("Text Displays")]
    [SerializeField] private TMP_Text modeDisplay;
    [SerializeField] private TMP_Text difficultyDisplay;

    [Header("Dependencies")]
    [SerializeField] private WeldingStatsRecorder statsRecorder;
    [SerializeField] private SphereSpawner spawner;
    [SerializeField] private GameSettings gameSettings;


    private bool isUIVisible = true;

    void Start()
    {
      
        toggleUIAction.action.performed += _ => ToggleUI();

        difficultyDisplay.text = gameSettings.dificultad;
        modeDisplay.text = gameSettings.modo;

        mainUICanvas.SetActive(true);
    }


    public void ToggleUI()
    {
        isUIVisible = !isUIVisible;
        mainUICanvas.SetActive(isUIVisible);
    }

    void OnDestroy()
    {
        // Desuscribir eventos
        toggleUIAction.action.performed -= _ => ToggleUI();
    }

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


    }
}