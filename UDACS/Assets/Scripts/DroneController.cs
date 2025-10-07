using System;
using TMPro;
using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class DroneController : MonoBehaviour
{
    public TMP_Text heightText;
    public TMP_Text speedText;
    public TMP_Text verticalSpeedText;
    public TMP_Text modeText;

    public float deadzone = 0.1f;
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
    float modeAxis;

    float[] maxYawRate = { 100f, 200f, 250f }; // Degrees/s
    float[] maxRollPitchRate = { 2f, 5f, 240f };
    float[] turnAcceleration = { 5f, 10f, 200f };
    float[] maxTiltAngle = { 5f, 15f }; // Normal & Sport max roll

    float[] acceleration = { 40f, 80f, 100f };
    float[] maxHorizontalMoveSpeed = { 9f, 17f };
    float[] maxVerticalMoveSpeed = { 6f, 9f, 55f };

    float minHeight = 1f; // minimum height above ground

    float cameraOffset = 0f;
    float flightPauseRTHInput;
    byte rthTimer;
    bool rthPressed;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        rthTimer = 0;
    }

    bool modePushed = false;
    void Update()
    {
        // Initialize Controls
        flightPauseRTHInput = PlayerInteraction.FlightPauseRTH.ReadValue<float>();

        if (PlayerInteraction.ControlSet == "Gamepad")
        {
            // Pause and return to home input handling
            if (flightPauseRTHInput == 1)
            {
                if (rthTimer > 60f * Time.deltaTime)
                {
                    rthPressed = false;
                    RTH();
                }
                else
                {
                    rthPressed = true;
                    rthTimer++;
                }
            }
            else if (rthPressed)
            {
                FlightPause();
                rthPressed = false;
            }
            else rthTimer = 0;
        }
        else if (PlayerInteraction.ControlSet == "KeyboardMouse")
        {
            if (flightPauseRTHInput == 1) FlightPause();
            else if (flightPauseRTHInput == -1) RTH();
        }

        if (PlayerInteraction.LeftStick == null) return;
        // Initialize Stick Inputs
        Vector2 leftStickRaw = PlayerInteraction.LeftStick.ReadValue<Vector2>();
        Vector2 rightStickRaw = PlayerInteraction.RightStick.ReadValue<Vector2>();
        float deadzone = 0.1f;

        // Apply deadzones
        leftStick = new(
            (Mathf.Abs(leftStickRaw.x) < deadzone) ? 0f : Mathf.Sign(leftStickRaw.x) * ((Mathf.Abs(leftStickRaw.x) - deadzone) / (1f - deadzone)),
            (Mathf.Abs(leftStickRaw.y) < deadzone) ? 0f : Mathf.Sign(leftStickRaw.y) * ((Mathf.Abs(leftStickRaw.y) - deadzone) / (1f - deadzone))
        );
        rightStick = new(
            (Mathf.Abs(rightStickRaw.x) < deadzone) ? 0f : Mathf.Sign(rightStickRaw.x) * ((Mathf.Abs(rightStickRaw.x) - deadzone) / (1f - deadzone)),
            (Mathf.Abs(rightStickRaw.y) < deadzone) ? 0f : Mathf.Sign(rightStickRaw.y) * ((Mathf.Abs(rightStickRaw.y) - deadzone) / (1f - deadzone))
        );

        modeAxis = PlayerInteraction.ModeToggle.ReadValue<float>();

        // Mode toggle
        switch (modeAxis)
        {
            case -1f:
                if (modePushed != true)
                {
                    mode = (Mode)Mathf.Clamp((int)(mode - 1), 0, 2);
                    modePushed = true;
                }
                break;
            case 1f:
                if (modePushed != true)
                {
                    mode = (Mode)Mathf.Clamp((int)(mode + 1), 0, 2);
                    modePushed = true;
                }
                break;
            default: modePushed = false;
                break;
        }
        char[] modeCharacters = { 'N', 'S', 'M' };
        modeText.text = modeCharacters[(int)mode].ToString();

        // Stabilize camera
        Vector3 mainCameraTransform = Camera.main.transform.localEulerAngles;
        Camera.main.transform.localEulerAngles = (mode == Mode.Manual) ? new Vector3(mainCameraTransform.x + Mathf.DeltaAngle(mainCameraTransform.x, cameraOffset = Mathf.Clamp(cameraOffset + PlayerInteraction.CameraAdjust.ReadValue<float>() / 4f, -90, 90)), mainCameraTransform.y, mainCameraTransform.z) : (Mathf.Abs(Mathf.DeltaAngle(mainCameraTransform.x, -transform.eulerAngles.x)) < 1f) ? new Vector3(-transform.eulerAngles.x, mainCameraTransform.y, mainCameraTransform.z) : new Vector3(mainCameraTransform.x + Mathf.DeltaAngle(mainCameraTransform.x, -transform.eulerAngles.x) * Time.deltaTime * 20f, mainCameraTransform.y, mainCameraTransform.z);
    }

    Quaternion? levelRotation = null;
    float levellingProgress = 0f;
    float? speedLastFrame;
    float? verticalSpeedLastFrame;
    bool wasManual;

    private void FixedUpdate()
    {
        // LeftStick X == Yaw
        // LeftStick Y == Collective
        // RightStick X == Roll
        // RightStick Y == Pitch

        // Prevent slow fall effect for non manual modes
        rigidBody.useGravity = mode == Mode.Manual;

        // Detect switch from manual mode and set values for transition
        if (wasManual && mode != Mode.Manual)
        {
            rigidBody.angularVelocity = Vector3.zero;

            levelRotation = Quaternion.Euler(0, rigidBody.rotation.eulerAngles.y, 0);
            levellingProgress = 0f;
            wasManual = false;
        }

        // Level the drone at a set speed
        if (levelRotation.HasValue)
        {
            levellingProgress += Time.fixedDeltaTime / 3f;
            rigidBody.MoveRotation(Quaternion.Slerp(rigidBody.rotation, levelRotation.Value, levellingProgress));

            if (Quaternion.Angle(rigidBody.rotation, levelRotation.Value) < 0.1f) levelRotation = null;
        }
        else if (mode == Mode.Normal || mode == Mode.Sport)
        {
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
            wasManual = true;

            int modeId = 2; // Manual mode

            // Smoothly move toward target angular velocity in world space
            rigidBody.angularVelocity = transform.TransformDirection(Vector3.MoveTowards(
                transform.InverseTransformDirection(rigidBody.angularVelocity),
                new Vector3(rightStick.y * maxRollPitchRate[modeId], leftStick.x * maxYawRate[modeId], -rightStick.x * maxRollPitchRate[modeId]) * Mathf.Deg2Rad,
                turnAcceleration[modeId] * Mathf.Deg2Rad * Time.fixedDeltaTime
            ));

            // Thrust along local up axis
            float velocityDifference = leftStick.y * maxVerticalMoveSpeed[modeId] - transform.InverseTransformDirection(rigidBody.linearVelocity).y;

            rigidBody.AddForce(transform.up * (rigidBody.mass * Physics.gravity.magnitude + Mathf.Clamp(velocityDifference * rigidBody.mass, -acceleration[modeId], acceleration[modeId])), ForceMode.Force);
        }

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 999f))
        {
            heightText.text = $"H: {hit.distance:0.0}m";
            // If too close to the ground, zero out downward velocity
            if (hit.distance <= minHeight && rigidBody.linearVelocity.y < 0) rigidBody.linearVelocity = new Vector3(rigidBody.linearVelocity.x, 0f, rigidBody.linearVelocity.z);
        }
        else heightText.text = "H: XXXXm";

        // Calculate absolute speed
        if (speedLastFrame == null) speedLastFrame = 0f;
        speedText.text = $"{(Mathf.Sqrt(Mathf.Pow(rigidBody.linearVelocity.z, 2) + Mathf.Pow(rigidBody.linearVelocity.x, 2)) - speedLastFrame / 50) * 3.6:0.0}kph";
        speedLastFrame = Mathf.Sqrt(Mathf.Pow(rigidBody.linearVelocity.z, 2) + Mathf.Pow(rigidBody.linearVelocity.x, 2));

        // Calculate vertical speed
        if (verticalSpeedLastFrame == null) verticalSpeedLastFrame = 0f;
        verticalSpeedText.text = $"{(rigidBody.linearVelocity.y - verticalSpeedLastFrame / 50) * 3.6:0.0}kph";
        speedLastFrame = rigidBody.linearVelocity.y;
    }

    void RTH()
    {

    }

    void FlightPause()
    {

    }
}