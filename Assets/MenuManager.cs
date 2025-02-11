using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject MenuPanel;
    public GameObject opcionesPanel;
    public GameObject creditosPanel;
    public GameObject dificultadPanel;
    public string CambiodeScena;
    private bool menuisactive = true;
    public static string dificultadSeleccionada;

    public void Play()
    {
        dificultadPanel.SetActive(true);
        MenuPanel.SetActive(false);
    }

    public void SeleccionarDificultad(string dificultad)
    {
        dificultadSeleccionada = dificultad;
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

    public void Salir()
    {
        Application.Quit();
    }
}

