using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private PlayerInputManager inputManager;

    private List<PlayerInput> _inputList = new List<PlayerInput>();
    private int _controllerIndex;

    void Start()
    {
        inputManager.onPlayerJoined += InputPlayerJoined;
    }

    private void OnDestroy()
    {
        inputManager.onPlayerJoined -= InputPlayerJoined;
    }

    private void InputPlayerJoined(PlayerInput playerInput)
    {
        _inputList.Add(playerInput);
        playerInput.SwitchCurrentControlScheme("Controller",Gamepad.all[_controllerIndex]);
        _controllerIndex++;
    }

    void Update()
    {
        
    }

    
}
