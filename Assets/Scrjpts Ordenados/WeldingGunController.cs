using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeldingGunController : MonoBehaviour
{
    [Header("Configuración de la pistola")]
    [SerializeField] private Transform weldingGunTip1;
    [SerializeField] private Transform weldingGunTip2;
    [SerializeField] private GameObject weldingGunModel1;
    [SerializeField] private GameObject weldingGunModel2;
    [SerializeField] private GameSettings gameSettings;

    public Transform CurrentGunTip { get; private set; }

    void Start() => SetModeSettings();

    public void SwitchGun(Transform newGunTip, GameObject newGunModel, GameObject oldGunModel)
    {
        CurrentGunTip = newGunTip;
        newGunModel.SetActive(true);
        oldGunModel.SetActive(false);
    }

    private void SetModeSettings()
    {
        if (gameSettings.modo == "SMAW")
        {
            currentGunTip = weldingGunTip1;
            weldingGunModel1.SetActive(true);
            weldingGunModel2.SetActive(false);
        }
        else
        {
            currentGunTip = weldingGunTip2;
            weldingGunModel2.SetActive(true);
            weldingGunModel1.SetActive(false);
        }
    }
}
