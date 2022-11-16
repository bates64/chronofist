using General;
using Input;

public class InputManager : Singleton<InputManager> {
    public enum Mode {
        None,
        Player,
        Interface
    }

    public Mode initialMode = Mode.None;
    private InterfaceInputs _interfaceInput;

    private PlayerInputs _playerInput;

    protected override void init() {
        _playerInput = new PlayerInputs();
        _interfaceInput = new InterfaceInputs();
        SetMode(initialMode);
    }

    public static void SetMode(Mode mode) {
        Instance.setMode(mode);
    }

    private void setMode(Mode mode) {
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

    #region Properties

    public static PlayerInputs PlayerInput => Instance._playerInput;
    public static InterfaceInputs InterfaceInput => Instance._interfaceInput;

    public Mode mode { get; private set; } = Mode.None;

    #endregion
}
