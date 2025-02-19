using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SphereSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private InputActionReference activateAction;
    [SerializeField] private GameObject grabbableSphere;
    [SerializeField] private float spawnInterval = 0.3f;

    private bool isSpawning = false;
    private List<GameObject> spawnedSpheres = new List<GameObject>();
    private Coroutine spawningCoroutine;
    private WeldingStatsRecorder statsRecorder;

    public List<GameObject> SpawnedSpheres => spawnedSpheres;

    void Start()
    {
        statsRecorder = GetComponent<WeldingStatsRecorder>();
        activateAction.action.performed += StartSpawning;
        activateAction.action.canceled += StopSpawning;
    }

    private IEnumerator SpawnSpheres()
    {
        while (isSpawning)
        {
            GameObject sphere = Instantiate(grabbableSphere, transform.position, Quaternion.identity);
            spawnedSpheres.Add(sphere);

            float angle = Vector3.Angle(transform.forward, Vector3.up);
            float arcLength = Physics.Raycast(transform.position, -transform.up, out RaycastHit hit) ? hit.distance : 0f;
            float speed = CalculateSpeed();

            statsRecorder.RecordStats(angle, arcLength, speed);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private float CalculateSpeed()
    {
        return spawnedSpheres.Count > 1 ?
            Vector3.Distance(
                spawnedSpheres[^1].transform.position,
                spawnedSpheres[^2].transform.position
            ) / spawnInterval : 0;
    }

    private void StartSpawning(InputAction.CallbackContext context) => StartCoroutine(SpawnSpheres());
    private void StopSpawning(InputAction.CallbackContext context) => isSpawning = false;

    void OnDestroy()
    {
        activateAction.action.performed -= StartSpawning;
        activateAction.action.canceled -= StopSpawning;
    }
}