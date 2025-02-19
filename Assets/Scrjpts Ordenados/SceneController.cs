using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneControllers : MonoBehaviour
{
    private ARPlaneVisibilityController _planeController;
    private WeldingGunController _gunController;
    private SphereSpawner _sphereSpawner;
    private PrecisionCalculator _precisionCalculator;
    private UIManager _uiManager;

    void Start()
    {
        _planeController = GetComponent<ARPlaneVisibilityController>();
        _gunController = GetComponent<WeldingGunController>();
        _precisionCalculator = GetComponent<PrecisionCalculator>();
        _uiManager = GetComponent<UIManager>();
        _sphereSpawner = GetComponent<SphereSpawner>();

        _uiManager.Initialize(_gunController, _precisionCalculator, _sphereSpawner);
    }
}
