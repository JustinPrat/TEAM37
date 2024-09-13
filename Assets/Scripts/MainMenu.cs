using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private SpriteRenderer camFlash;

    private IA_Movement playerControls;

    public void PlayGame()
    {
        camFlash.DOKill();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        camFlash.DOKill();
        Application.Quit();
    }

    private void Start()
    {
        Color color = camFlash.color;
        color.a = 0.1f;
        camFlash.DOColor(color, 2).SetLoops(-1, LoopType.Yoyo);

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
