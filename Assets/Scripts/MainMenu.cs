using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private IA_Movement playerControls;

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void Start()
    {
        playerControls = new IA_Movement();
        playerControls.Enable();
        playerControls.Player.Spawn.performed += Spawn_performed;
    }

    private void OnDisable()
    {
        playerControls.Player.Spawn.performed -= Spawn_performed;
    }

    private void Spawn_performed(InputAction.CallbackContext obj)
    {
        PlayGame();
    }

}
