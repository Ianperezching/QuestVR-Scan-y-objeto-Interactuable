using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;

public class VRMenuController : MonoBehaviour
{
    public GameObject menuPanel; // Panel del menú de opciones
    public Slider effectsSlider, ambientSlider, musicSlider; // Sliders de audio
    public Toggle muteToggle; // Opción para silenciar
    public Button exitButton, cancelButton, confirmExitButton;
    public GameObject exitConfirmationPanel;

    private bool isMenuOpen = false;

    void Start()
    {
        menuPanel.SetActive(false);
        exitConfirmationPanel.SetActive(false);
        exitButton.onClick.AddListener(ShowExitConfirmation);
        cancelButton.onClick.AddListener(CancelExit);
        confirmExitButton.onClick.AddListener(ExitToMainMenu);
        muteToggle.onValueChanged.AddListener(ToggleMute);

        effectsSlider.onValueChanged.AddListener(SetEffectsVolume);
        ambientSlider.onValueChanged.AddListener(SetAmbientVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
    }

    void Update()
    {
        //if (OVRInput.GetDown(OVRInput.Button.Start)) // Botón en control derecho
        //{
        //    ToggleMenu();
        //}
    }

    public void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;
        menuPanel.SetActive(isMenuOpen);
        Time.timeScale = isMenuOpen ? 0f : 1f; // La simulación sigue activa
    }

    void ShowExitConfirmation()
    {
        exitConfirmationPanel.SetActive(true);
    }

    void CancelExit()
    {
        exitConfirmationPanel.SetActive(false);
    }

    void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    void ToggleMute(bool isMuted)
    {
        AudioListener.volume = isMuted ? 0f : 1f;
    }

    void SetEffectsVolume(float volume)
    {
        // Aquí ajustas el volumen de efectos de sonido
    }

    void SetAmbientVolume(float volume)
    {
        // Aquí ajustas el volumen del ambiente
    }

    void SetMusicVolume(float volume)
    {
        // Aquí ajustas el volumen de la música
    }
}
