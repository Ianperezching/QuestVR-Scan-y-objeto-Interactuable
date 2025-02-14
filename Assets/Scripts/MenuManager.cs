using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject MenuPanel;
    public GameObject opcionesPanel;
    public GameObject creditosPanel;
    public GameObject dificultadPanel;
    public GameObject modoPanel;
    public string CambiodeScena;
    private bool menuisactive = true;
    public GameSettings gameSettings;

    public void Play()
    {
        dificultadPanel.SetActive(true);
        MenuPanel.SetActive(false);
    }

    public void SeleccionarDificultad(string dificultad)
    {
        gameSettings.dificultad = dificultad;
        dificultadPanel.SetActive(false);
        modoPanel.SetActive(true);
    }

    public void SeleccionarModo(string modo)
    {
        gameSettings.modo = modo;
        SceneManager.LoadScene(CambiodeScena);
    }

    public void AbrirOpciones()
    {
        if (menuisactive)
        {
            MenuPanel.SetActive(false);
            opcionesPanel.SetActive(true);
        }
        else
        {
            MenuPanel.SetActive(true);
            opcionesPanel.SetActive(false);
        }
        menuisactive = !menuisactive;
    }

    public void AbrirCreditos()
    {
        if (menuisactive)
        {
            MenuPanel.SetActive(false);
            creditosPanel.SetActive(true);
        }
        else
        {
            MenuPanel.SetActive(true);
            creditosPanel.SetActive(false);
        }
        menuisactive = !menuisactive;
    }

    public void irMenu()
    {
        MenuPanel.SetActive(true);
        creditosPanel.SetActive(false);
        opcionesPanel.SetActive(false);
        dificultadPanel.SetActive(false);
        modoPanel.SetActive(false);
        menuisactive = true;
    }

    public void Salir()
    {
        Application.Quit();
    }
}