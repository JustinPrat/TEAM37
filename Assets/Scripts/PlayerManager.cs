using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private PlayerInputManager inputManager;
    [SerializeField] private List<TextMeshProUGUI> textList;

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
        playerInput.gameObject.tag = "Player" + _controllerIndex;
        _controllerIndex++;

        if (_controllerIndex >= 2)
        {
            Player firstPlayer = _inputList[0].GetComponent<Player>();
            Player secondPlayer = _inputList[1].GetComponent<Player>();
            firstPlayer.OtherPlayer = secondPlayer;
            secondPlayer.OtherPlayer = firstPlayer;

            firstPlayer.TextMeshProUGUI = textList[0];
            secondPlayer.TextMeshProUGUI = textList[1];
        }
    }

    void Update()
    {
        
    }

    
}
