using TMPro;
using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class DroneController : MonoBehaviour
{
    public TMP_Text heightText;
    public enum Mode
    {
        Normal,
        Sport,
        Manual
    }
    [SerializeField]
    Mode mode = Mode.Normal;

    Rigidbody rigidBody;

    Vector2 leftStick;
    Vector2 rightStick;

    float[] maxYawRate = { 120f, 240f }; // Degrees/s
    float[] maxRollPitchRate = { 5f, 10f };
    float[] turnAcceleration = { 5f, 10f };
    float[] maxTiltAngle = { 5f, 15f }; // Normal & Sport max roll

    float[] acceleration = { 40f, 80f };
    float[] maxHorizontalMoveSpeed = { 8f, 16f };
    float[] maxVerticalMoveSpeed = { 6f, 9f };

    float minHeight = 1f; // minimum height above ground

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }
    void Update()
    {
        // Initialize Inputs
        leftStick = PlayerInteraction.LeftStick.ReadValue<Vector2>();
        rightStick = PlayerInteraction.RightStick.ReadValue<Vector2>();

        // Stabilize camera
        Vector3 mainCameraTransform = Camera.main.transform.localEulerAngles;
        Camera.main.transform.localEulerAngles = new Vector3(-transform.eulerAngles.x, mainCameraTransform.y, mainCameraTransform.z);
    }
    private void FixedUpdate()
    {
        // LeftStick X == Yaw
        // LeftStick Y == Collective
        // RightStick X == Roll
        // RightStick Y == Pitch
        if (mode == Mode.Normal || mode == Mode.Sport)
        {
            // Prevent slow fall effect
            rigidBody.useGravity = false;

            // Set params
            byte modeId = (byte)(mode == Mode.Normal ? 0 : 1);

            // Yaw
            rigidBody.MoveRotation(rigidBody.rotation * Quaternion.Euler(0f, leftStick.x * maxYawRate[modeId] * Time.fixedDeltaTime, 0f));

            float targetPitch = rightStick.y * maxTiltAngle[modeId]; // Pitch
            float targetRoll = -rightStick.x * maxTiltAngle[modeId]; // Roll

            // Smoothly tilt drone
            rigidBody.MoveRotation(Quaternion.Slerp(rigidBody.rotation, Quaternion.Euler(targetPitch, rigidBody.rotation.eulerAngles.y, targetRoll), maxRollPitchRate[modeId] * Time.fixedDeltaTime));

            // Horizontal movement
            Quaternion currentYaw = Quaternion.Euler(0f, transform.eulerAngles.y, 0f); // Get yaw
            Vector3 forward = currentYaw * Vector3.forward;
            Vector3 right = currentYaw * Vector3.right;

            // Convert stick input to local yaw and scale to move speed
            Vector3 horizontal = Vector3.ClampMagnitude((currentYaw * Vector3.forward * rightStick.y) + (currentYaw * Vector3.right * rightStick.x), 1f) * maxHorizontalMoveSpeed[modeId];
            horizontal.y = rigidBody.linearVelocity.y; // Preserve vertical velocity
            rigidBody.linearVelocity = horizontal;

            // Collective
            rigidBody.linearVelocity = new Vector3(rigidBody.linearVelocity.x, Mathf.MoveTowards(rigidBody.linearVelocity.y, leftStick.y * maxVerticalMoveSpeed[modeId], acceleration[modeId] * Time.fixedDeltaTime), rigidBody.linearVelocity.z);
        }
        else if (mode == Mode.Manual)
        {
            // Apply gravity again
            rigidBody.useGravity = false;

            // Smooth acceleration toward target angular velocity
            rigidBody.angularVelocity = Vector3.MoveTowards(rigidBody.angularVelocity, new Vector3(-rightStick.y * maxRollPitchRate[0], leftStick.x * maxYawRate[0], -rightStick.x * maxRollPitchRate[0]), turnAcceleration[0] * Time.fixedDeltaTime);
        }

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1000f))
        {
            heightText.text = $"H: {hit.distance:0.0}m";
            // If too close to the ground, zero out downward velocity
            if (hit.distance <= minHeight && rigidBody.linearVelocity.y < 0) rigidBody.linearVelocity = new Vector3(rigidBody.linearVelocity.x, 0f, rigidBody.linearVelocity.z);
        }
        else heightText.text = "H: XXXXm";
    }
}