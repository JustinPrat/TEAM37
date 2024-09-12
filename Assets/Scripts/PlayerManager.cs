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

    [SerializeField] private List<MonoBehaviour> scriptsToStart;

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

            firstPlayer.enabled = true;
            secondPlayer.enabled = true;

            firstPlayer.OtherPlayer = secondPlayer;
            secondPlayer.OtherPlayer = firstPlayer;

            firstPlayer.TextMeshProObject = _textList[0];
            secondPlayer.TextMeshProObject = _textList[1];

            firstPlayer.Loupiote = _loupioteList[0];
            secondPlayer.Loupiote = _loupioteList[1];

            secondPlayer.PlayerAnimator.SetTrigger("secondPlayer");

            foreach (MonoBehaviour script in scriptsToStart)
            {
                script.enabled = true;
            }
        }
    }

    void Update()
    {
        
    }

    
}
