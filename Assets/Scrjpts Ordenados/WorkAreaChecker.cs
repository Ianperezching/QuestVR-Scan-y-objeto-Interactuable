using UnityEngine;
using System.Collections.Generic;

public class WorkAreaChecker : MonoBehaviour
{
    public bool IsInWorkArea { get; private set; } = false;
    public List<GameObject> SpheresInArea { get; } = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WeldingGun"))
        {
            IsInWorkArea = true;
            Debug.Log("[WorkArea] Pistola DENTRO del área");
        }

        if (other.CompareTag("WeldingSphere"))
        {
            SpheresInArea.Add(other.gameObject);
            Debug.Log($"[WorkArea] Esfera {other.name} AGREGADA. Total: {SpheresInArea.Count}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("WeldingGun"))
        {
            IsInWorkArea = false;
            Debug.Log("[WorkArea] Pistola FUERA del área");
        }

        if (other.CompareTag("WeldingSphere"))
        {
            SpheresInArea.Remove(other.gameObject);
            Debug.Log($"[WorkArea] Esfera {other.name} REMOVIDA. Restantes: {SpheresInArea.Count}");
            Destroy(other.gameObject);
        }
    }
}