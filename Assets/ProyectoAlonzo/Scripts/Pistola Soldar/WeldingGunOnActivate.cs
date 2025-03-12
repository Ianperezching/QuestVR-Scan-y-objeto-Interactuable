using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WeldingGunOnActivate : MonoBehaviour
{
    public ParticleSystem sparkEffect; // Efecto de partículas para las chispas
    public ParticleSystem fireEffect; // Efecto de partículas para el fuego
    public Transform spawnPoint; // Punto de origen de las chispas y el fuego
    public AcidDecalSpawner acidDecalSpawner; // Referencia al AcidDecalSpawner

    private ParticleSystem currentFire; // Referencia al efecto de fuego actual
    private ParticleSystem currentSparks; // Referencia al efecto de chispas actual
    private bool isTriggerPressed = false; // Estado del gatillo
    private bool isNearMetal = false; // Indica si el fuego está cerca de un objeto de metal
    private Vector3 metalContactPoint; // Punto de contacto con el metal

    void Start()
    {
        // Obtén el componente XRGrabInteractable y añade listeners para los eventos de activación y desactivación
        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        grabbable.activated.AddListener(StartWelding);
        grabbable.deactivated.AddListener(StopWelding);
    }

    void Update()
    {
        // Si el gatillo está presionado y el fuego está activo, mueve el fuego y verifica si está cerca de un objeto de metal
        if (isTriggerPressed && currentFire != null)
        {
            // Mueve el fuego junto con la pistola
            currentFire.transform.position = spawnPoint.position;
            currentFire.transform.rotation = spawnPoint.rotation;

            // Verifica si el fuego está cerca de un objeto de metal
            CheckForMetal();
        }

        // Si el gatillo está presionado y estamos cerca de metal, aplica el ácido
        if (isTriggerPressed && isNearMetal)
        {
            acidDecalSpawner.SpawnAcidDecal(metalContactPoint); // Genera el decal de ácido en el punto de contacto
        }
    }

    public void StartWelding(ActivateEventArgs arg)
    {
        // Activa el efecto de fuego
        isTriggerPressed = true;
        currentFire = Instantiate(fireEffect, spawnPoint.position, spawnPoint.rotation);
        currentFire.Play(); // Reproduce el efecto de fuego

        // Verifica si el fuego está cerca de un objeto de metal
        CheckForMetal();
    }

    public void StopWelding(DeactivateEventArgs arg)
    {
        // Detiene el efecto de fuego y las chispas
        isTriggerPressed = false;
        isNearMetal = false;

        if (currentFire != null)
        {
            currentFire.Stop(); // Detiene el fuego
            Destroy(currentFire.gameObject, currentFire.main.duration); // Destruye el fuego después de que termine
        }

        if (currentSparks != null)
        {
            currentSparks.Stop(); // Detiene las chispas
            Destroy(currentSparks.gameObject, currentSparks.main.duration); // Destruye las chispas después de que terminen
        }
    }

    private void CheckForMetal()
    {
        // Lanza un rayo desde el spawnPoint para detectar objetos de metal
        RaycastHit hit;
        if (Physics.Raycast(spawnPoint.position, spawnPoint.forward, out hit, 1.0f)) // Ajusta la distancia del rayo según sea necesario
        {
            if (hit.collider.CompareTag("Metal")) // Verifica si el objeto tiene el tag "Metal"
            {
                isNearMetal = true;
                metalContactPoint = hit.point; // Guarda el punto de contacto con el metal

                // Aplica el ácido al cubo
                CubeWeldingController cubeController = hit.collider.GetComponent<CubeWeldingController>();
                if (cubeController != null)
                {
                    cubeController.ApplyAcid(); // Marca el cubo como que tiene ácido aplicado
                }

                // Activa las chispas en la posición de colisión
                if (currentSparks == null)
                {
                    currentSparks = Instantiate(sparkEffect, hit.point, Quaternion.identity);
                    currentSparks.Play(); // Reproduce el efecto de chispas
                }
                else
                {
                    // Mueve las chispas al nuevo punto de colisión
                    currentSparks.transform.position = hit.point;
                }
            }
            else
            {
                // Si el objeto no es de metal, desactiva las chispas
                isNearMetal = false;
                if (currentSparks != null)
                {
                    currentSparks.Stop(); // Detiene las chispas
                    Destroy(currentSparks.gameObject, currentSparks.main.duration); // Destruye las chispas después de que terminen
                    currentSparks = null;
                }
            }
        }
        else
        {
            // Si no hay colisión, desactiva las chispas
            isNearMetal = false;
            if (currentSparks != null)
            {
                currentSparks.Stop(); // Detiene las chispas
                Destroy(currentSparks.gameObject, currentSparks.main.duration); // Destruye las chispas después de que terminen
                currentSparks = null;
            }
        }
    }
}