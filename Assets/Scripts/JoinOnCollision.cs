using UnityEngine;

public class JoinOnCollision : MonoBehaviour
{
    public string targetTag = "Joinable"; 

    void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag(targetTag))
        {
          
            Rigidbody otherRb = collision.gameObject.GetComponent<Rigidbody>();

            if (otherRb != null)
            {
              
                FixedJoint joint = gameObject.AddComponent<FixedJoint>();

               
                joint.connectedBody = otherRb;

                Debug.Log($"Objetos unidos: {gameObject.name} y {collision.gameObject.name}");
            }
            else
            {
                Debug.LogError("El objeto colisionado no tiene un Rigidbody.");
            }
        }
    }
}
