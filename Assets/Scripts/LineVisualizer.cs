using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineVisualizer : MonoBehaviour
{
    public Transform[] LineaArreglo; // Puntos que forman la LineaArreglo
    public Transform PuntoA;        // Punto A de la LineaTrayectoria
    public Transform PuntoB;        // Punto B de la LineaTrayectoria
    [Range(0,90)]
    public float tolerance;
    void OnDrawGizmos()
    {
        // Dibujar la LineaArreglo
        if (LineaArreglo != null && LineaArreglo.Length > 0)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < LineaArreglo.Length; i++)
            {
                if (LineaArreglo[i] != null)
                {
                    // Dibujar esfera en cada punto
                    Gizmos.DrawSphere(LineaArreglo[i].position, 0.01f);

                    // Dibujar líneas entre puntos consecutivos
                    if (i < LineaArreglo.Length - 1 && LineaArreglo[i + 1] != null)
                    {
                        Gizmos.DrawLine(LineaArreglo[i].position, LineaArreglo[i + 1].position);
                    }
                }
            }
        }

        // Dibujar la LineaTrayectoria
        if (PuntoA != null && PuntoB != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(PuntoA.position + Vector3.up * tolerance, PuntoB.position + Vector3.up * tolerance);
            Gizmos.DrawLine(PuntoA.position + Vector3.up * -tolerance, PuntoB.position + Vector3.up * -tolerance);


            Gizmos.color = Color.red;
            Gizmos.DrawLine(PuntoA.position, PuntoB.position);
             // Dibujar esferas en los puntos A y B
            //Gizmos.DrawSphere(PuntoA.position, tolerance);
            //Gizmos.DrawSphere(PuntoB.position, tolerance);
        }

        Gizmos.color = Color.white;
        Vector3 direccionAB = (PuntoB.position - PuntoA.position).normalized;

        for (int i = 0; i < LineaArreglo.Length; i++)
        {
            // Vector desde A al punto actual
            Vector3 direccionAP = (LineaArreglo[i].position - PuntoA.position);

            // Proyección del vector AP sobre AB para calcular la distancia perpendicular
            float distanciaPerpendicular = Vector3.Cross(direccionAB, direccionAP).magnitude / direccionAB.magnitude;

            Vector3 cross = Vector3.Cross(direccionAB, direccionAP).normalized;

            Gizmos.DrawLine(LineaArreglo[i].position, LineaArreglo[i].position + cross * distanciaPerpendicular);
        }

    }

    public static float CalcularSimilitud(Vector3[] lineaArreglo, Vector3 puntoA, Vector3 puntoB, float tolerance = 1f)
    {
        if (lineaArreglo == null || lineaArreglo.Length == 0)
        {
            Debug.LogError("LineaArreglo está vacía o es nula.");
            return 0f;
        }

        if (Vector3.Distance(puntoA, puntoB) < Mathf.Epsilon)
        {
            Debug.LogError("Los puntos A y B son demasiado cercanos. No se puede definir una línea.");
            return 0f;
        }

        int puntosAlineados = 0;

        // Vector director de la línea AB
        Vector3 direccionAB = (puntoB - puntoA).normalized;

        foreach (Vector3 punto in lineaArreglo)
        {
            // Vector desde A al punto actual
            Vector3 direccionAP = (punto - puntoA);

            // Proyección del vector AP sobre AB para calcular la distancia perpendicular
            float distanciaPerpendicular = Vector3.Cross(direccionAB, direccionAP).magnitude / direccionAB.magnitude;

            // Si la distancia perpendicular está dentro de la tolerancia, consideramos el punto alineado
            if (distanciaPerpendicular <= tolerance)
            {
                puntosAlineados++;
            }
        }

        // Calcular el porcentaje de puntos alineados
        return (float)puntosAlineados / lineaArreglo.Length * 100f;
    }
    public static float CalcularSimilitudAngulo(Vector3[] lineaArreglo, Vector3 puntoA, Vector3 puntoB, float tolerance = 1f)
    {
        if (lineaArreglo == null || lineaArreglo.Length == 0)
        {
            Debug.LogError("LineaArreglo está vacía o es nula.");
            return 0f;
        }

        if (Vector3.Distance(puntoA, puntoB) < Mathf.Epsilon)
        {
            Debug.LogError("Los puntos A y B son demasiado cercanos. No se puede definir una línea.");
            return 0f;
        }

        int puntosAlineados = 0;

        // Vector director de la línea AB
        Vector3 direccionAB = (puntoB - puntoA).normalized;

        foreach (Vector3 punto in lineaArreglo)
        {
            // Vector desde A al punto actual
            Vector3 direccionAP = (puntoB - punto).normalized;

            // Proyección del vector AP sobre AB para calcular la distancia perpendicular
            //float distanciaPerpendicular = Vector3.Cross(direccionAB, direccionAP).magnitude / direccionAB.magnitude;
            float angle = Vector3.Angle(direccionAB, direccionAP);
            Debug.Log("angle" + angle);
            // Si la distancia perpendicular está dentro de la tolerancia, consideramos el punto alineado
            if (angle >=0 && angle <= tolerance)
            {
                puntosAlineados++;
            }
        }

        // Calcular el porcentaje de puntos alineados
        return (float)puntosAlineados / lineaArreglo.Length * 100f;
    }
}
