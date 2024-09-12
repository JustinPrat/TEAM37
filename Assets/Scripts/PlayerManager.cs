using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private PlayerInputManager _inputManager;
    [SerializeField] private List<TextMeshProUGUI> _textList;
    [SerializeField] private List<SpriteRenderer> _loupioteList;

    private List<PlayerInput> _inputList = new List<PlayerInput>();
    private int _controllerIndex;

    void Start()
    {
        _inputManager.onPlayerJoined += InputPlayerJoined;
    }

    private void OnDestroy()
    {
        _inputManager.onPlayerJoined -= InputPlayerJoined;
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

            firstPlayer.TextMeshProUGUI = _textList[0];
            secondPlayer.TextMeshProUGUI = _textList[1];

            firstPlayer.Loupiote = _loupioteList[0];
            secondPlayer.Loupiote = _loupioteList[1];
        }
    }

    void Update()
    {
        
    }

    
}
