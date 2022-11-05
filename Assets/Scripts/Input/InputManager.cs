﻿using General;
using Input;

public class InputManager : Singleton<InputManager> {
    public enum Mode {
        None,
        Player,
        Interface,
    }

    private PlayerInputs _playerInput;
    private InterfaceInputs _interfaceInput;

    #region Properties

    public static PlayerInputs PlayerInput => Instance._playerInput;
    public static InterfaceInputs InterfaceInput => Instance._interfaceInput;

    public Mode mode { get; private set; } = Mode.None;

    #endregion

    protected override void init() {
        _playerInput = new PlayerInputs();
        _interfaceInput = new InterfaceInputs();
        SetMode(Mode.Player);
    }

    public void SetMode(Mode mode) {
        switch (this.mode) {
            case Mode.Player:
                _playerInput.Disable();
                break;
            case Mode.Interface:
                _interfaceInput.Disable();
                break;
        }

        this.mode = mode;

        switch (mode) {
            case Mode.Player:
                _playerInput.Enable();
                break;
            case Mode.Interface:
                _interfaceInput.Enable();
                break;
        }
    }
}
