using UnityEngine;

public class SceneCoordinator : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private ARPlaneVisibilityController planeController;
    [SerializeField] private WeldingGunController gunController;
    [SerializeField] private SphereSpawner sphereSpawner;
    [SerializeField] private PrecisionCalculator precisionCalculator;
    [SerializeField] private UIManager uiManager;

    void Start()
    {
        uiManager.Initialize(gunController, precisionCalculator, sphereSpawner);
    }
}