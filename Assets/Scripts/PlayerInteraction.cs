using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public InputActionAsset actions;
    public PlayerInput playerInput;

    public static InputAction Move;
    public static InputAction Look;
    public static InputAction Jump;
    public static InputAction Sprint;
    public static InputAction Pause;

    void Start()
    {
        playerInput.actions = actions;
        playerInput.SwitchCurrentActionMap("Drone");
    }

    void Update()
    {
        Move = playerInput.actions["Move"];
        Look = playerInput.actions["Look"];
        Jump = playerInput.actions["Jump"];
        Sprint = playerInput.actions["Sprint"];
        Pause = playerInput.actions["Pause"];
    }
}
