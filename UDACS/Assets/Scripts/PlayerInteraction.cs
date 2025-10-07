using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public InputActionAsset actions;
    public PlayerInput playerInput;

    public static InputAction LeftStick;
    public static InputAction RightStick;
    public static InputAction Pause;
    public static InputAction ModeToggle;
    public static InputAction FlightPauseRTH;
    public static InputAction RecordPhoto;
    public static InputAction StartStop;
    public static InputAction CameraAdjust;
    public static InputAction Deploy;

    void Start()
    {
        playerInput.actions = actions;
        playerInput.SwitchCurrentActionMap("Drone");
    }

    void Update()
    {
        RightStick = playerInput.actions["Right Stick"];
        LeftStick = playerInput.actions["Left Stick"];
        Pause = playerInput.actions["Pause"];
        ModeToggle = playerInput.actions["Mode Toggle"];
        FlightPauseRTH = playerInput.actions["RTH FlightPause"];
        RecordPhoto = playerInput.actions["Record Photo"];
        StartStop = playerInput.actions["Start Stop"];
    }
}
