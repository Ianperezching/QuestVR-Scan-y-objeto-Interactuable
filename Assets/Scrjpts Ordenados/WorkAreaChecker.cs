using UnityEngine;

public class WorkAreaChecker : MonoBehaviour
{
    public bool IsInWorkArea { get; private set; } = false;

    // Detecta cuando la pistola entra al área
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WeldingGun"))
        {
            IsInWorkArea = true;
        }
    }

    // Detecta cuando la pistola sale del área
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("WeldingGun"))
        {
            IsInWorkArea = false;
        }
    }
}
