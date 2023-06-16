using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public Camera mainCamera;
    public Canvas TopBar;
    public Canvas About;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void OpenAbout()
    {
        About.gameObject.SetActive(true);
        mainCamera.GetComponent<SystemDraw>().enabled = false;
    }

    public void CloseAbout()
    {
        About.gameObject.SetActive(false);
        mainCamera.GetComponent<SystemDraw>().enabled = true;
    }

    public void OpenGithub()
    {
        Application.OpenURL("https://github.com/sebbett/ED-Backviz");
    }
}
