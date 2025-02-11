using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject MenuPanel;
    public GameObject opcionesPanel;
    public GameObject creditosPanel;
    public string CambiodeScena;
    private bool menuisactive = true;
    public void Play(string CambiodeScena)
    {
        SceneManager.LoadScene(CambiodeScena);
    }

    public void AbrirOpciones()
    {
        if (menuisactive == true)
        {
            MenuPanel.SetActive(false);
            opcionesPanel.SetActive(true);
            menuisactive = false;
        }
        else
        {
            MenuPanel.SetActive(true);
            opcionesPanel.SetActive(false);
            menuisactive = true;
        }
    }

    public void AbrirCreditos()
    {
        if (menuisactive == true)
        {
            MenuPanel.SetActive(false);
            creditosPanel.SetActive(true);
            menuisactive = false;
        }
        else
        {
            MenuPanel.SetActive(true);
            creditosPanel.SetActive(false);
            menuisactive = true;
        }
    }

    public void Salir()
    {
        Application.Quit();
    }
}

