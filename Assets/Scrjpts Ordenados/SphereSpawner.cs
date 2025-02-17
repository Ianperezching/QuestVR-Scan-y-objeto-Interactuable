using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

public class SphereSpawner : MonoBehaviour
{
    [SerializeField] private InputActionReference _activateAction;
    [SerializeField] private GameObject _GrabbableSphere;
    [SerializeField] private Transform currentGunTip;

    private bool _isSpawning = false;
    private List<GameObject> spawnedSpheres = new List<GameObject>();

    void Start()
    {
        _activateAction.action.performed += StartSpawning;
        _activateAction.action.canceled += StopSpawning;
    }

    private void StartSpawning(InputAction.CallbackContext obj) => StartCoroutine(SpawnWeldingSpheres());
    private void StopSpawning(InputAction.CallbackContext obj) => _isSpawning = false;

    private IEnumerator SpawnWeldingSpheres()
    {
        while (_isSpawning)
        {
            if (_GrabbableSphere != null && currentGunTip != null)
            {
                GameObject sphere = Instantiate(_GrabbableSphere, currentGunTip.position, Quaternion.identity);
                spawnedSpheres.Add(sphere);
                RecordStatistics();
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

}
