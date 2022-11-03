using System.Collections;
using System.Collections.Generic;
using General;
using Input;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    private readonly PlayerInputs _playerInput = new PlayerInputs();

    #region Properties

    public static PlayerInputs PlayerInput => Instance._playerInput;

    #endregion
    
    protected override void init()
    {
        _playerInput.Init();
    }
}
