using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PrecisionCalculators : MonoBehaviour
{
    [Header("Referencias")]
    public Transform PuntoA;        // Punto inicial de la l�nea gu�a
    public Transform PuntoB;        // Punto final de la l�nea gu�a
    public TMP_Text ResultadoTexto; // TextMeshPro para mostrar el resultado
    public float Tolerancia = 0.1f; // Distancia m�xima aceptada para precisi�n

    private List<GameObject> esferasCreadas = new List<GameObject>(); // Lista de esferas

    /// <summary>
    /// Agregar esfera a la lista cuando se crea.
    /// </summary>
    public void AgregarEsfera(GameObject esfera)
    {
        esferasCreadas.Add(esfera);
    }

    /// <summary>
    /// Calcular la precisi�n de las esferas respecto a la l�nea gu�a.
    /// </summary>
    public void CalcularPrecision()
    {
        if (esferasCreadas.Count == 0)
        {
            ResultadoTexto.text = "No hay esferas para calcular.";
            return;
        }

        int dentroDeTolerancia = 0;
        Vector3 direccionLinea = (PuntoB.position - PuntoA.position).normalized;

        foreach (GameObject esfera in esferasCreadas)
        {
            if (esfera == null) continue;

            Vector3 direccionEsfera = esfera.transform.position - PuntoA.position;
            float distanciaPerpendicular = Vector3.Cross(direccionLinea, direccionEsfera).magnitude / direccionLinea.magnitude;

            if (distanciaPerpendicular <= Tolerancia)
            {
                dentroDeTolerancia++;
            }
        }

        float porcentajePrecision = (float)dentroDeTolerancia / esferasCreadas.Count * 100f;
        ResultadoTexto.text = $"Precisi�n: {porcentajePrecision:F2}%";
    }
}