using UnityEngine;

public class WeldingGunController : MonoBehaviour
{
    [Header("Gun Configuration")]
    [SerializeField] private Transform weldingGunTip1;
    [SerializeField] private Transform weldingGunTip2;
    [SerializeField] private GameObject weldingGunModel1;
    [SerializeField] private GameObject weldingGunModel2;
    [SerializeField] private GameSettings gameSettings;

    
    public Transform CurrentGunTip { get; private set; }
    public Transform GunTip1 => weldingGunTip1; 
    public Transform GunTip2 => weldingGunTip2; 

    void Start() => SetInitialGun();

    public void SwitchGun(bool isGun1)
    {
        CurrentGunTip = isGun1 ? weldingGunTip1 : weldingGunTip2;
        weldingGunModel1.SetActive(isGun1);
        weldingGunModel2.SetActive(!isGun1);
    }

    private void SetInitialGun() => SwitchGun(gameSettings.modo == "SMAW");
}