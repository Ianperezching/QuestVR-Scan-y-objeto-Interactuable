using UnityEngine;
using UnityEngine.InputSystem;

public class WeldingGunOnActivate : MonoBehaviour
{
    [Header("Efectos de Partículas")]
    public ParticleSystem sparkEffect;  // Chispas para soldadura
    public ParticleSystem fireEffect;   // Fuego base constante

    [Header("Configuración")]
    public Transform spawnPoint;        // Punto de origen de efectos
    [SerializeField] private float rayDistance = 1.0f; // Distancia de detección

    [Header("Controles")]
    [SerializeField] private InputActionReference triggerAction; // Acción del gatillo

    private ParticleSystem currentFire;   // Referencia al fuego activo
    private ParticleSystem currentSparks; // Referencia a chispas activas
    private bool isTriggerActive = false; // Estado del gatillo

    void Start()
    {
        // Configurar eventos de entrada
        triggerAction.action.performed += _ => StartWelding();
        triggerAction.action.canceled += _ => StopWelding();
    }

    void Update()
    {
        if (isTriggerActive)
        {
            UpdateFirePosition();
            CheckForJoinable();
        }
    }

    void OnDestroy()
    {
        // Limpiar eventos de entrada
        triggerAction.action.performed -= _ => StartWelding();
        triggerAction.action.canceled -= _ => StopWelding();
    }

    // ========== LÓGICA PRINCIPAL ========== //
    private void StartWelding()
    {
        isTriggerActive = true;

        // Activar fuego constante
        if (currentFire == null)
        {
            currentFire = Instantiate(fireEffect, spawnPoint);
            currentFire.Play();
        }
    }

    private void StopWelding()
    {
        isTriggerActive = false;

        // Detener efectos
        if (currentFire != null)
        {
            currentFire.Stop();
            Destroy(currentFire.gameObject, 2f);
        }
        DestroySparks();
    }

    // ========== FUNCIONES AUXILIARES ========== //
    private void UpdateFirePosition()
    {
        // Mantener el fuego en la punta de la pistola
        if (currentFire != null)
        {
            currentFire.transform.position = spawnPoint.position;
            currentFire.transform.rotation = spawnPoint.rotation;
        }
    }

    private void CheckForJoinable()
    {
        RaycastHit hit;
        bool hitJoinable = Physics.Raycast(
            spawnPoint.position,
            spawnPoint.forward,
            out hit,
            rayDistance
        ) && hit.collider.CompareTag("Joinable");

        ManageSparks(hitJoinable, hit.point);
    }

    private void ManageSparks(bool shouldSpawn, Vector3 position)
    {
        if (shouldSpawn)
        {
            if (currentSparks == null)
            {
                currentSparks = Instantiate(sparkEffect, position, Quaternion.identity);
                currentSparks.Play();
            }
            else
            {
                currentSparks.transform.position = position;
            }
        }
        else
        {
            DestroySparks();
        }
    }

    private void DestroySparks()
    {
        if (currentSparks != null)
        {
            currentSparks.Stop();
            Destroy(currentSparks.gameObject, currentSparks.main.duration);
            currentSparks = null;
        }
    }

    // ========== DEBUG ========== //
    void OnDrawGizmos()
    {
        if (isTriggerActive)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(spawnPoint.position, spawnPoint.forward * rayDistance);
        }
    }
}