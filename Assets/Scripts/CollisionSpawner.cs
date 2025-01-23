using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSpawner : MonoBehaviour
{
    [Header("Prefab a Instanciar")]
    public GameObject prefabToSpawn;  // Prefab definido desde el Inspector

    [Header("Etiqueta del Objeto con el que colisionar")]
    public string targetTag = "Target";  // Etiqueta del objeto con el que debe colisionar

    private void OnCollisionEnter(Collision collision)
    {
        // Verifica si el objeto con el que colisiona tiene la etiqueta definida
        if (collision.gameObject.CompareTag(targetTag))
        {
            // Obtiene la posici�n del punto de contacto
            Vector3 collisionPoint = collision.contacts[0].point;

            // Instancia el prefab en la posici�n de la colisi�n con rotaci�n por defecto
            Instantiate(prefabToSpawn, collisionPoint, Quaternion.identity);

            // Destruye ambos objetos involucrados en la colisi�n
            Destroy(collision.gameObject);  // Destruye el objeto con el que colisiona
            Destroy(gameObject);            // Destruye el propio objeto
        }
    }
}
