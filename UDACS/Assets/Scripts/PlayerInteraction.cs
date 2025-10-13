using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public InputActionAsset actions;
    public PlayerInput playerInput;

    public static string ControlSet;
    public static InputAction LeftStick;
    public static InputAction RightStick;
    public static InputAction FlightPauseRTH;
    public static InputAction Pause;
    public static InputAction RecordPhoto;
    public static InputAction ModeToggle;
    public static InputAction StartStop;
    public static InputAction CameraAdjust;
    public static InputAction Deploy;
    public static InputAction CameraToggle;

    void Start()
    {
        playerInput.actions = actions;
        playerInput.SwitchCurrentActionMap("Drone");
    }

    void Update()
    {
        // Expose the current control scheme
        ControlSet = playerInput.currentControlScheme;

        // Bind inputs to public variables
        RightStick = playerInput.actions["Right Stick"];
        LeftStick = playerInput.actions["Left Stick"];
        FlightPauseRTH = playerInput.actions["RTH FlightPause"];
        Pause = playerInput.actions["Pause"];
        RecordPhoto = playerInput.actions["Record Photo"];
        ModeToggle = playerInput.actions["Mode Toggle"];
        StartStop = playerInput.actions["Start Stop"];
        CameraAdjust = playerInput.actions["CameraAdjust"];
        Deploy = playerInput.actions["Deploy"];
        CameraToggle = playerInput.actions["CameraToggle"];
    }
}
