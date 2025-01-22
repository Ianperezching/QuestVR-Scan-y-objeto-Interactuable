using UnityEngine;
using UnityEditor;



[CustomEditor(typeof(LineVisualizer))]
public class LineVisualizerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LineVisualizer visualizer = (LineVisualizer)target;

        if (GUILayout.Button("Calcular Similitud"))
        {
            if (visualizer.LineaArreglo != null && visualizer.PuntoA != null && visualizer.PuntoB != null)
            {
                Vector3[] puntos = new Vector3[visualizer.LineaArreglo.Length];
                for (int i = 0; i < visualizer.LineaArreglo.Length; i++)
                {
                    puntos[i] = visualizer.LineaArreglo[i].position;
                }

                float similitud = LineVisualizer.CalcularSimilitudAngulo(
                    puntos,
                    visualizer.PuntoA.position,
                    visualizer.PuntoB.position,
                    visualizer.tolerance
                );

                Debug.Log("Porcentaje de similitud: " + similitud + "%");
            }
            else
            {
                Debug.LogError("Asegúrate de asignar todos los valores en el inspector.");
            }
        }
    }
}
