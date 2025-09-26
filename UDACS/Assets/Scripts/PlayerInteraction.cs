using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public InputActionAsset actions;
    public PlayerInput playerInput;

    public static InputAction LeftStick;
    public static InputAction RightStick;
    public static InputAction Pause;
    // public static InputAction Jump;
    // public static InputAction Sprint;

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
        // Sprint = playerInput.actions["Sprint"];
        // Pause = playerInput.actions["Pause"];
    }
}
