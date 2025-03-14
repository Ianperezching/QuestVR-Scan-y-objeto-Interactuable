using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PrecisionCalculators : MonoBehaviour
{
    [Header("Referencias")]
    public Transform PuntoA;
    public Transform PuntoB;
    public TMP_Text ResultadoTexto;
    public float Tolerancia = 0.1f;
    public PistolaSphereCastMerge pistola;

    [Header("Parámetros Óptimos")]
    public float velocidadOptima = 0.15f;
    public float anguloOptimo = 30f;
    public float estabilidadOptima = 5f;

    private List<GameObject> esferasCreadas = new List<GameObject>();

    public void AgregarEsfera(GameObject esfera)
    {
        esferasCreadas.Add(esfera);
    }

    public void CalcularPrecisionFinal()
    {
        if (esferasCreadas.Count == 0)
        {
            ResultadoTexto.text = "No hay soldaduras realizadas";
            return;
        }

        // Precisión de alineación
        int dentroTolerancia = CalcularAlineacion();
        float precisionAlineacion = (float)dentroTolerancia / esferasCreadas.Count * 100f;

        // Precisión técnica
        float puntajeTotal = CalcularPuntajeTotal(precisionAlineacion);

        MostrarResultado(puntajeTotal, precisionAlineacion);
    }

    int CalcularAlineacion()
    {
        int dentro = 0;
        Vector3 direccionLinea = (PuntoB.position - PuntoA.position).normalized;

        foreach (GameObject SoldaduraDecal in esferasCreadas)
        {
            if (SoldaduraDecal == null) continue;

            Vector3 direccionEsfera = SoldaduraDecal.transform.position - PuntoA.position;
            float distanciaPerpendicular = Vector3.Cross(direccionLinea, direccionEsfera).magnitude / direccionLinea.magnitude;

            if (distanciaPerpendicular <= Tolerancia) dentro++;
        }
        return dentro;
    }

    float CalcularPuntajeTotal(float precisionAlin)
    {
        float puntajeVelocidad = Mathf.Clamp01(1 - Mathf.Abs(pistola.velocidadActual - velocidadOptima) / velocidadOptima);
        float puntajeAngulo = Mathf.Clamp01(1 - Mathf.Abs(pistola.anguloActual - anguloOptimo) / anguloOptimo);
        float puntajeEstabilidad = Mathf.Clamp01(1 - pistola.estabilidadActual / estabilidadOptima);

        return (precisionAlin / 100f * 0.4f) +
              (puntajeVelocidad * 0.2f) +
              (puntajeAngulo * 0.2f) +
              (puntajeEstabilidad * 0.2f);
    }

    void MostrarResultado(float puntajeTotal, float precisionAlin)
    {
        ResultadoTexto.text = $"<size=120%><b>PRECISIÓN TOTAL: {puntajeTotal * 100:F1}%</b></size>\n\n" +
                             $"Alineación: {precisionAlin:F1}%\n" +
                             $"Velocidad: {pistola.velocidadActual:F2} m/s (Óptimo: {velocidadOptima:F2})\n" +
                             $"Ángulo: {pistola.anguloActual:F1}° (Óptimo: {anguloOptimo:F1})\n" +
                             $"Estabilidad: {pistola.estabilidadActual:F2}° (Óptimo: ≤{estabilidadOptima:F1})";
    }
}