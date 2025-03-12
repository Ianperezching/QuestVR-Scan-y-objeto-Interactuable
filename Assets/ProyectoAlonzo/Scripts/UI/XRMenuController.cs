using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class XRMenuController : MonoBehaviour
{
    public GameObject menuCanvas; // El Canvas completo del menú
    public GameObject panelOpciones; // Panel de opciones
    public GameObject panelCreditos; // Panel de créditos

    private bool isMenuVisible = false;

    void Update()
    {
        // Abrir/cerrar el menú con la tecla "M" (puedes cambiar la tecla)
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMenu();
        }
    }

    void ToggleMenu()
    {
        isMenuVisible = !isMenuVisible;
        menuCanvas.SetActive(isMenuVisible); // Activa o desactiva el Canvas completo
    }

    // Método para el botón "Iniciar"
    public void OnIniciarPressed()
    {
        Debug.Log("Iniciar juego...");
        SceneManager.LoadScene("MainVR"); // Cambia "NombreDeTuEscenaDeJuego" por el nombre de tu escena
    }

    // Método para el botón "Opciones"
    public void OnOpcionesPressed()
    {
        Debug.Log("Mostrando opciones...");
        panelOpciones.SetActive(true); // Muestra el panel de opciones
        panelCreditos.SetActive(false); // Oculta el panel de créditos
    }

    // Método para el botón "Créditos"
    public void OnCreditosPressed()
    {
        Debug.Log("Mostrando créditos...");
        panelCreditos.SetActive(true); // Muestra el panel de créditos
        panelOpciones.SetActive(false); // Oculta el panel de opciones
    }

    // Método para el botón "Salir"
    public void OnSalirPressed()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit(); // Cierra la aplicación (solo funciona en builds, no en el editor)
    }
}