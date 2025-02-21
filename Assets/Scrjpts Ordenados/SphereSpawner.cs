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
    [SerializeField] private WeldingGunController gunController;

    [Header("Work Area Restriction")]
    [SerializeField] private WorkAreaChecker workAreaChecker;

    private bool isSpawning = false;
    private List<GameObject> spawnedSpheres = new List<GameObject>();
    private Coroutine spawningCoroutine;
    private WeldingStatsRecorder statsRecorder;
    private Vector3 lastSpawnPosition;
    private float lastSpawnTime;

    public List<GameObject> SpawnedSpheres => spawnedSpheres;

    void Start()
    {
        statsRecorder = GetComponent<WeldingStatsRecorder>();

        if (gunController == null || activateAction == null || grabbableSphere == null || workAreaChecker == null)
            Debug.LogError("¡Faltan referencias en el Inspector!");

        activateAction.action.performed += StartSpawning;
        activateAction.action.canceled += StopSpawning;
    }

    private IEnumerator SpawnSpheres()
    {
        isSpawning = true;
        lastSpawnPosition = gunController.CurrentGunTip.position;
        lastSpawnTime = Time.time;

        while (isSpawning)
        {
            if (workAreaChecker.IsInWorkArea)
            {
                Transform currentTip = gunController.CurrentGunTip;
                GameObject sphere = Instantiate(grabbableSphere, currentTip.position, currentTip.rotation);
                spawnedSpheres.Add(sphere);

                float angle = Vector3.Angle(currentTip.forward, Vector3.up);
                float arcLength = Physics.Raycast(currentTip.position, -currentTip.up, out RaycastHit hit)
                    ? hit.distance
                    : 0f;
                float speed = CalculateSpeed();

                statsRecorder.RecordStats(angle, arcLength, speed);

                lastSpawnPosition = currentTip.position;
                lastSpawnTime = Time.time;
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private float CalculateSpeed()
    {
        if (spawnedSpheres.Count < 1) return 0;

        float distance = Vector3.Distance(
            gunController.CurrentGunTip.position,
            lastSpawnPosition
        );

        float timeDiff = Time.time - lastSpawnTime;
        return timeDiff > 0.001f ? distance / timeDiff : 0;
    }

    private void StartSpawning(InputAction.CallbackContext context)
    {
        if (!isSpawning)
        {
            if (spawningCoroutine != null)
                StopCoroutine(spawningCoroutine);

            spawningCoroutine = StartCoroutine(SpawnSpheres());
        }
    }

    private void StopSpawning(InputAction.CallbackContext context) => isSpawning = false;

    void OnDestroy()
    {
        if (activateAction != null)
        {
            activateAction.action.performed -= StartSpawning;
            activateAction.action.canceled -= StopSpawning;
        }
    }

    void OnDrawGizmos()
    {
        if (gunController != null && gunController.CurrentGunTip != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(gunController.CurrentGunTip.position, 0.02f);
        }
    }
}