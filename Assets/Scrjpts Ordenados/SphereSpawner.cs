using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SphereSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private InputActionReference activateAction; // Acción de entrada (ej: gatillo VR)
    [SerializeField] private GameObject grabbableSphere;          // Prefab de la esfera
    [SerializeField] private float spawnInterval = 0.3f;          // Intervalo entre spawns
    [SerializeField] private WeldingGunController gunController;  // Controlador de la pistola

    private bool isSpawning = false;
    private List<GameObject> spawnedSpheres = new List<GameObject>();
    private Coroutine spawningCoroutine;
    private WeldingStatsRecorder statsRecorder;
    private Vector3 lastSpawnPosition;
    private float lastSpawnTime;

    public List<GameObject> SpawnedSpheres => spawnedSpheres;     // Acceso público a las esferas

    [Header("Work Area Restriction")]
    [SerializeField] private WorkAreaChecker workAreaChecker;

    void Start()
    {
        statsRecorder = GetComponent<WeldingStatsRecorder>();

        // Verificar referencias críticas
        if (gunController == null || activateAction == null || grabbableSphere == null)
            Debug.LogError("¡Faltan referencias en el Inspector!");

        // Suscribir eventos de entrada
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
           
            // Instanciar esfera en la punta activa de la pistola
            Transform currentTip = gunController.CurrentGunTip;
            GameObject sphere = Instantiate(
                grabbableSphere,
                currentTip.position,
                currentTip.rotation
            );
            spawnedSpheres.Add(sphere);

            // Calcular métricas
            float angle = Vector3.Angle(currentTip.forward, Vector3.up);
            float arcLength = Physics.Raycast(currentTip.position, -currentTip.up, out RaycastHit hit)
                ? hit.distance
                : 0f;
            float speed = CalculateSpeed();

            statsRecorder.RecordStats(angle, arcLength, speed);

            // Actualizar para el próximo cálculo
            lastSpawnPosition = currentTip.position;
            lastSpawnTime = Time.time;

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
        return timeDiff > 0.001f ? distance / timeDiff : 0; // Evitar división por cero
    }

    private void StartSpawning(InputAction.CallbackContext context)
    {
        if (!isSpawning)
        {
            // Detener corrutina anterior si existe
            if (spawningCoroutine != null)
                StopCoroutine(spawningCoroutine);

            spawningCoroutine = StartCoroutine(SpawnSpheres());
        }
    }

    private void StopSpawning(InputAction.CallbackContext context) => isSpawning = false;

    void OnDestroy()
    {
        // Desuscribir eventos al destruir el objeto
        if (activateAction != null)
        {
            activateAction.action.performed -= StartSpawning;
            activateAction.action.canceled -= StopSpawning;
        }
    }

    // Visualizar punto de spawn en el Editor
    void OnDrawGizmos()
    {
        if (gunController != null && gunController.CurrentGunTip != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(gunController.CurrentGunTip.position, 0.02f);
        }
    }
}