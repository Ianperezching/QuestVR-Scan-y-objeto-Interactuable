using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class WeldingGunOnActivates : MonoBehaviour
{
    [Header("Particle Effects")]
    public ParticleSystem sparkEffect;
    public ParticleSystem fireEffect;

    [Header("Decal Settings")]
    public GameObject decalPrefab;
    public float decalOffset = 0.01f;

    [Header("References")]
    public Transform spawnPoint;
    [SerializeField] private float rayDistance = 1.0f;

    [Header("Input Settings")]
    [SerializeField] private InputActionReference triggerAction;

    private ParticleSystem currentFire;
    private ParticleSystem currentSparks;
    private List<GameObject> allDecals = new List<GameObject>();
    private bool isTriggerActive = false;

    void Start()
    {
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
        triggerAction.action.performed -= _ => StartWelding();
        triggerAction.action.canceled -= _ => StopWelding();
    }

    private void StartWelding()
    {
        isTriggerActive = true;

        if (currentFire == null)
        {
            currentFire = Instantiate(fireEffect, spawnPoint);
            currentFire.Play();
        }
    }

    private void StopWelding()
    {
        isTriggerActive = false;

        if (currentFire != null)
        {
            currentFire.Stop();
            Destroy(currentFire.gameObject, 2f);
        }
        DestroySparks();
    }

    private void UpdateFirePosition()
    {
        if (currentFire != null)
        {
            // Usar posición y rotación GLOBAL del spawnPoint
            currentFire.transform.position = spawnPoint.position;
            currentFire.transform.rotation = spawnPoint.rotation;

            // Si el fuego mira en dirección contraria, invertir eje Z
            // currentFire.transform.rotation *= Quaternion.Euler(0, 180, 0);
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

        if (hitJoinable)
        {
            ManageSparks(true, hit.point);
            CreateDecal(hit);
        }
        else
        {
            ManageSparks(false, Vector3.zero);
        }
    }

    private void CreateDecal(RaycastHit hit)
    {
        GameObject newDecal = Instantiate(
            decalPrefab,
            hit.point + hit.normal * decalOffset,
            Quaternion.LookRotation(-hit.normal)
        );

        // Configurar decal
        newDecal.transform.SetParent(hit.collider.transform);
        float randomScale = Random.Range(0.8f, 1.2f);
        newDecal.transform.localScale = Vector3.one * randomScale;
        allDecals.Add(newDecal);
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

    // Método para limpiar todos los decales (opcional)
    public void ClearAllDecals()
    {
        foreach (GameObject decal in allDecals)
        {
            if (decal != null) Destroy(decal);
        }
        allDecals.Clear();
    }

    void OnDrawGizmos()
    {
        if (isTriggerActive)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(spawnPoint.position, spawnPoint.forward * rayDistance);
        }
    }
}