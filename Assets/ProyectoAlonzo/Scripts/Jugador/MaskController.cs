using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MaskController : MonoBehaviour
{
    public Transform headTransform; // Transform de la cabeza del jugador
    public float proximityDistance = 0.3f; // Distancia para detectar si la máscara está cerca de la cara
    public Vector3 maskOffset = new Vector3(0, 0, 0.1f); // Ajuste de posición de la máscara
    public Transform desiredPosition; // Posición donde debe ajustarse la máscara
    public float moveSpeed = 5f; // Velocidad de movimiento al colocarse

    private XRGrabInteractable grabInteractable;
    private Rigidbody maskRigidbody;
    private bool isMovingToFace = false;
    private bool isAttached = false; // Indica si la máscara está en la cara

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        maskRigidbody = GetComponent<Rigidbody>();

        // Suscribirse a eventos de agarre y soltar
        grabInteractable.onSelectEntered.AddListener(OnGrabbed);
        grabInteractable.onSelectExited.AddListener(OnReleased);
    }

    void Update()
    {
        if (isMovingToFace)
        {
            // Mover suavemente la máscara hacia la cara
            transform.position = Vector3.MoveTowards(transform.position, desiredPosition.position, moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredPosition.rotation, moveSpeed * Time.deltaTime);

            // Si la máscara llega a la posición deseada, la fijamos
            if (Vector3.Distance(transform.position, desiredPosition.position) < 0.01f)
            {
                AttachToFace();
            }
            
        }
        print("Distance " + Vector3.Distance(transform.position, headTransform.position));
    }

    private void OnGrabbed(XRBaseInteractor interactor)
    {
        // Si la máscara está en la cara, permitir soltarla nuevamente
        if (isAttached)
        {
            DetachFromFace();
        }

        // Restaurar la física para poder mover la máscara libremente
        isMovingToFace = false;
        maskRigidbody.isKinematic = false;
    }

    private void OnReleased(XRBaseInteractor interactor)
    {
        // Si la máscara está cerca de la cabeza, activar el movimiento hacia la cara
        if (Vector3.Distance(transform.position, headTransform.position) < proximityDistance)
        {
            print("Attach");
            isMovingToFace = true;
        }
        else
        {
            print("Detach1");
            DetachFromFace();
            
        }
    }

    private void AttachToFace()
    {
        isMovingToFace = false;
        isAttached = true;
        maskRigidbody.isKinematic = true; // Hacer la máscara cinemática para que no caiga
        transform.position = desiredPosition.position;
        transform.rotation = desiredPosition.rotation;
        transform.parent = desiredPosition.parent; // Anclar a la cabeza
    }

    private void DetachFromFace()
    {
        print("Detach2");
        isAttached = false;
        transform.parent = null; // Desanclar la máscara de la cabeza
        maskRigidbody.isKinematic = false; // Restaurar la física
    }
}
