using UnityEngine;
using System.Collections.Generic;

public class WorkAreaChecker : MonoBehaviour
{
    public bool IsInWorkArea { get; private set; } = false;
    public List<GameObject> SpheresInArea { get; } = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WeldingGun"))
            IsInWorkArea = true;

        if (other.CompareTag("WeldingSphere"))
            SpheresInArea.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("WeldingGun"))
            IsInWorkArea = false;

        if (other.CompareTag("WeldingSphere"))
        {
            SpheresInArea.Remove(other.gameObject);
            Destroy(other.gameObject); // Destruir al salir del área
        }
    }
}