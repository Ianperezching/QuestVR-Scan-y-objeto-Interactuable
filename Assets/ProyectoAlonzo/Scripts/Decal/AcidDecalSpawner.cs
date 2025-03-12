using UnityEngine;

public class AcidDecalSpawner : MonoBehaviour
{
    public GameObject decalPrefab; // Prefab del decal con Decal Projector
    public float spawnInterval = 0.1f; // Intervalo de tiempo entre cada spawn de decal

    private float lastSpawnTime = 0f;

    public void SpawnAcidDecal(Vector3 contactPosition)
    {
        // Verifica si ha pasado el tiempo suficiente desde el último spawn
        if (Time.time - lastSpawnTime >= spawnInterval)
        {
            // Instancia el decal en la posición de contacto
            GameObject decal = Instantiate(decalPrefab, contactPosition, Quaternion.identity);

            // Asegúrate de que el decal esté correctamente orientado hacia la superficie
            decal.transform.forward = -decal.transform.up; // Ajusta según la orientación de tu prefab

            lastSpawnTime = Time.time; // Actualiza el tiempo del último spawn
        }
    }
}