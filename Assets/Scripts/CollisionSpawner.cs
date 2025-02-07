using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSpawner : MonoBehaviour
{
    [Header("Prefab a Instanciar")]
    public GameObject prefabToSpawn;  

    [Header("Etiqueta del Objeto con el que colisionar")]
    public string targetTag = "Target";  

    private void OnCollisionEnter(Collision collision)
    {
      
        if (collision.gameObject.CompareTag(targetTag))
        {
            
            Vector3 collisionPoint = collision.contacts[0].point;

            
            Instantiate(prefabToSpawn, collisionPoint, Quaternion.identity);

            
            Destroy(collision.gameObject); 
            Destroy(gameObject);            
        }
    }
}
