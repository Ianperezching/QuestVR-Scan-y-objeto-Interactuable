using UnityEngine;
using UnityEngine.UI;

public class WeldingUI : MonoBehaviour
{
    public PistolaSphereCastMerge weldingGun; // Referencia a la pistola de soldadura
    public Text voltageText; // Texto para mostrar el voltaje
    public Text wireSpeedText; // Texto para mostrar la velocidad del cable
    public Text totalTimeText; // Texto para mostrar el tiempo total
    public Text resultText; // Texto para mostrar el resultado final

    private float totalTime = 0f; // Tiempo total de soldadura
    private bool isWelding = false; // Indica si se está soldando

    void Update()
    {
        // Si la pistola está soldando, actualiza el tiempo total
        if (weldingGun.IsWelding())
        {
            if (!isWelding)
            {
                isWelding = true;
                totalTime = 0f; // Reinicia el tiempo si acaba de empezar a soldar
            }

            totalTime += Time.deltaTime;
            UpdateUI();
        }
        else
        {
            isWelding = false;
        }
    }

    void UpdateUI()
    {
        // Actualiza los textos de la UI con los valores correspondientes
        voltageText.text = "Voltaje: " + weldingGun.GetVoltage().ToString("F1") + " volts";
        wireSpeedText.text = "Velocidad de cable: " + weldingGun.GetWireSpeed().ToString("F0") + " ipm";
        totalTimeText.text = "Tiempo total: " + totalTime.ToString("F0") + " segundos";
        resultText.text = "Resultado final: " + weldingGun.GetWeldingResult();
    }
}