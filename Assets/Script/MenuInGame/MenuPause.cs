using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPause : MonoBehaviour
{
    [SerializeField] private GameObject jeu;
    [SerializeField] private GameObject menu;
    private bool isPaused = false; // Variable pour suivre l'état de pause

    void Update()
    {
        // Détecter l'appui sur la touche Échap
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused; // Basculer l'état de pause

        jeu.SetActive(!isPaused); // Activer/désactiver le jeu
        menu.SetActive(isPaused); // Activer/désactiver le menu

        // Gérer le curseur
        Cursor.visible = isPaused;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
    }
    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
