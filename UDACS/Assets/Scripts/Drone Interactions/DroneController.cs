using TMPro;
using UnityEngine;

/* DroneController Accessible Variables
 * this.invertRightStick
 * Allows you to toggle it true to invert the right control stick for weird people
 * 
*/ 

[RequireComponent (typeof(Rigidbody))]
public class DroneController : MonoBehaviour
{
    public TMP_Text heightText;
    public TMP_Text speedText;
    public TMP_Text verticalSpeedText;
    public TMP_Text distanceFromHomeText;
    public TMP_Text modeText;

    public bool droneOn = false;
    public bool invertRightStick = false;

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
    Vector3 startPosition;

    Vector2 leftStick;
    Vector2 rightStick;
    float modeAxis;

    float[] maxYawRate = { 100f, 200f, 400f }; // Degrees/s
    float[] maxRollPitchRate = { 5f, 10f, 500f };
    float[] maxTiltAngle = { 5f, 15f }; // Normal & Sport max roll

    float[] acceleration = { 40f, 80f, 200f };
    float[] maxHorizontalMoveSpeed = { 9f, 17f };
    float[] maxVerticalMoveSpeed = { 6f, 9f, 55f };

    float minHeight = 1f; // minimum height above ground

    public float upwardCameraPosition = 45f;

    float cameraOffset = 0f;
    byte cameraPosition = 0;
    float flightPauseRTHInput;
    bool manualCameraControl = false;
    float rthTimer;
    bool rthPressed;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        rthTimer = 0;
        startPosition = transform.position;
    }

    bool modePushed = false;
    bool rthTriggered;

    bool flightPause = false;

    float previousPropellerSpeed = 0f;
    bool pressedStartStop = false;
    void Update()
    {
        if (droneOn)
        {
            if (!Audio.instance.DroneSource.isPlaying)
            {
                switch (sequence)
                {
                    case 0:
                        if (mode == Mode.Manual)
                        {
                            sequence = 1;
                            break;
                        }
                        sequence++;
                        Audio.instance.PlayDroneSound("Start-up to idle");
                        break;
                    case 1:
                        sequence++;
                        Audio.instance.idleEngine.Play();
                        Audio.instance.fastEngine.Play();
                        break;
                    case 2:
                        float responsiveness = 5f;
                        // Compute propeller intensity and smooth to avoid jitter
                        float smoothPropellerSpeed = Mathf.Lerp(previousPropellerSpeed, Mathf.Clamp01(leftStick.magnitude + (rightStick.magnitude / 2f)), Time.deltaTime * responsiveness);
                        previousPropellerSpeed = smoothPropellerSpeed;

                        // Convert to decibels safely and apply to mixer
                        Audio.instance.mixer.SetFloat("HighRPM", Mathf.Lerp(-60f, -30f, smoothPropellerSpeed));
                        Audio.instance.mixer.SetFloat("IdleRPM", Mathf.Lerp(-50f, -30f, 1f - smoothPropellerSpeed));
                        break;
                }
            }
        }

        // Initialize Controls
        flightPauseRTHInput = PlayerInteraction.FlightPauseRTH.ReadValue<float>();

        if (PlayerInteraction.ControlSet == "Gamepad")
        {
            // Pause and return to home input handling
            if (flightPauseRTHInput == 1)
            {
                if (rthTimer > 1f)
                {
                    rthPressed = false;
                    rthTimer = 0f;
                    // Set mode to normal for control
                    mode = Mode.Normal;
                    rthTriggered = true;
                }
                else if (!rthTriggered)
                {
                    rthPressed = true;
                    rthTimer += Time.deltaTime;
                }
            }
            else if (rthPressed)
            {
                FlightPause();
                rthTriggered = false;
                rthPressed = false;
            }
            else rthTimer = 0;
        }
        else
        {
            if (flightPauseRTHInput == 1) FlightPause();
            else if (flightPauseRTHInput == -1)
            {
                mode = Mode.Normal;
                rthTriggered = true;
            }
        }

        if (PlayerInteraction.StartStop.ReadValue<float>() > 0.5f && !droneOn && !pressedStartStop)
        {
            pressedStartStop = true;
            if (mode == Mode.Manual)
            {
                droneOn = true;
            }
            else
            {
                Audio.instance.PlayDroneSound("Power_On");
                droneOn = true;
            }
        }
        else if (PlayerInteraction.StartStop.ReadValue<float>() > 0.5f && droneOn && mode == Mode.Manual && !pressedStartStop)
        {
            pressedStartStop = true;
            sequence = 0;
            droneOn = false;
        }
        else if (PlayerInteraction.StartStop.ReadValue<float>() < 0.1f) pressedStartStop = false;

        if (!droneOn)
        {
            flightPause = false;
            rthTriggered = false;
        }

        // Return to home
        if (rthTriggered)
        {
            flightPause = false;
            RTH();
        }

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
                    mode = (Mode)Mathf.Clamp((int)(mode + 1), 0, rthTriggered ? 1 : 2);
                    modePushed = true;
                }
                break;
            default: modePushed = false;
                break;
        }
        char[] modeCharacters = { 'N', 'S', 'M' };
        modeText.text = modeCharacters[(int)mode].ToString();

        float cameraAdjust = PlayerInteraction.CameraAdjust.ReadValue<float>();
        float[] cameraPositions = { 0, 90f, upwardCameraPosition };
        float[] stabilizedCameraPositions = { 0, 90f, upwardCameraPosition };

        // Toggle camera position
        if (PlayerInteraction.CameraToggle.triggered)
        {
            cameraPosition = (byte)((cameraPosition + 1) % 3);
            manualCameraControl = false;
            cameraOffset = cameraPositions[cameraPosition];
        }

        if ((cameraAdjust != 0 || modeAxis != 0) && manualCameraControl == false && mode == Mode.Manual)
        {
            cameraPosition = 0;
            manualCameraControl = true;
        }

        cameraOffset = Mathf.Clamp(cameraOffset + PlayerInteraction.CameraAdjust.ReadValue<float>() / 4f, -90, 90);

        // Stabilize camera
        Vector3 mainCameraTransform = Camera.main.transform.localEulerAngles;
        Camera.main.transform.localEulerAngles = (mode == Mode.Manual)
            ? new Vector3(mainCameraTransform.x + Mathf.DeltaAngle(mainCameraTransform.x, cameraOffset), mainCameraTransform.y, mainCameraTransform.z)
            : (Mathf.Abs(Mathf.DeltaAngle(mainCameraTransform.x, -transform.eulerAngles.x)) < 2f)
                ? new Vector3(-transform.eulerAngles.x + stabilizedCameraPositions[cameraPosition], mainCameraTransform.y, mainCameraTransform.z)
                : new Vector3(mainCameraTransform.x + Mathf.DeltaAngle(mainCameraTransform.x, -transform.eulerAngles.x + stabilizedCameraPositions[cameraPosition]) * Time.deltaTime * 20f, mainCameraTransform.y, mainCameraTransform.z);
    }

    Quaternion? levelRotation = null;
    float levellingProgress = 0f;
    bool wasManual;

    float distanceFromGround;

    byte sequence = 0;

    Vector2 previousLeftStick;
    Vector2 previousRightStick;
    private void FixedUpdate()
    {
        if (PlayerInteraction.LeftStick == null) return;
        // Initialize Stick Inputs
        Vector2 leftStickRaw = PlayerInteraction.LeftStick.ReadValue<Vector2>();
        Vector2 rightStickRaw = PlayerInteraction.RightStick.ReadValue<Vector2>();
        float deadzone = 0.1f;

        // Apply deadzones
        Vector2 deadzoneLeftStick = new(
            (Mathf.Abs(leftStickRaw.x) < deadzone) ? 0f : Mathf.Sign(leftStickRaw.x) * ((Mathf.Abs(leftStickRaw.x) - deadzone) / (1f - deadzone)),
            (Mathf.Abs(leftStickRaw.y) < deadzone) ? 0f : Mathf.Sign(leftStickRaw.y) * ((Mathf.Abs(leftStickRaw.y) - deadzone) / (1f - deadzone))
        );
        Vector2 deadzoneRightStick = new(
            (Mathf.Abs(rightStickRaw.x) < deadzone) ? 0f : Mathf.Sign(rightStickRaw.x) * ((Mathf.Abs(rightStickRaw.x) - deadzone) / (1f - deadzone)),
            (Mathf.Abs(rightStickRaw.y) < deadzone) ? 0f : Mathf.Sign(rightStickRaw.y) * ((Mathf.Abs(rightStickRaw.y) - deadzone) / (1f - deadzone))
        );

        // Apply deadzones
        if (droneOn && !rthTriggered)
        {
            if (!flightPause)
            {
                leftStick = deadzoneLeftStick;
                rightStick = deadzoneRightStick;
            }
            else
            {
                leftStick = Vector2.zero;
                rightStick = Vector2.zero;
            }
        }

        if (invertRightStick) rightStick = -rightStick;

        // Smooth both sticks over time to remove jitter
        leftStick = Vector2.Lerp(previousLeftStick, leftStick, Time.deltaTime * 10f);
        rightStick = Vector2.Lerp(previousRightStick, rightStick, Time.deltaTime * 10f);

        previousLeftStick = leftStick;
        previousRightStick = rightStick;

        // If inputs touched disable RTH
        if (deadzoneLeftStick != Vector2.zero || deadzoneRightStick != Vector2.zero) rthTriggered = false;

        if (!droneOn) sequence = 0;

        // LeftStick X == Yaw
        // LeftStick Y == Collective
        // RightStick X == Roll
        // RightStick Y == Pitch

        if (sequence == 2)
        {
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

                // If too close to the ground, zero out downward velocity
                if (distanceFromGround <= minHeight && rigidBody.linearVelocity.y < 0) rigidBody.linearVelocity = new Vector3(rigidBody.linearVelocity.x, 0f, rigidBody.linearVelocity.z);
            }
            else if (mode == Mode.Manual)
            {
                wasManual = true;

                int modeId = 2; // Manual mode

                // Smoothly move toward target angular velocity in world space
                rigidBody.angularVelocity = transform.TransformDirection(//Vector3.MoveTowards(
                    //transform.InverseTransformDirection(rigidBody.angularVelocity),
                    new Vector3(rightStick.y * maxRollPitchRate[modeId], leftStick.x * maxYawRate[modeId], -rightStick.x * maxRollPitchRate[modeId]) * Mathf.Deg2Rad//,
                    //turnAcceleration[modeId] * Mathf.Deg2Rad * Time.fixedDeltaTime
                /*)*/);

                // Thrust along local up axis
                float velocityDifference = leftStick.y * maxVerticalMoveSpeed[modeId] - transform.InverseTransformDirection(rigidBody.linearVelocity).y;

                rigidBody.AddForce(transform.up * (rigidBody.mass * Physics.gravity.magnitude + Mathf.Clamp(velocityDifference * rigidBody.mass, -acceleration[modeId], acceleration[modeId])), ForceMode.Force);
            }
        }
        else if (!droneOn)
        {
            Audio.instance.idleEngine.Stop();
            Audio.instance.fastEngine.Stop();
            rigidBody.useGravity = true;
        }

        RaycastHit[] hits = Physics.RaycastAll(transform.position, Vector3.down, 999f);
        heightText.text = "H: XXXXm";
        foreach (RaycastHit hit in hits)
        {
            if (!hit.collider.isTrigger)
            {
                distanceFromGround = hit.distance;
                heightText.text = $"H: {distanceFromGround:0.0}m";
                break;
            }
        }

        // Calculate absolute speed
        speedText.text = $"{Mathf.Sqrt(Mathf.Pow(rigidBody.linearVelocity.z, 2) + Mathf.Pow(rigidBody.linearVelocity.x, 2)) * 3.6:0.0}kph";

        // Calculate vertical speed
        verticalSpeedText.text = $"{rigidBody.linearVelocity.y * 3.6:0.0}kph";

        // Calculate distance to home
        distanceFromHomeText.text = $"D: {Vector3.Distance(startPosition, transform.position):0.0}m";
    }

    void RTH()
    {
        // Rotate to home
        leftStick.x = Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, Mathf.Atan2(startPosition.x - transform.position.x, startPosition.z - transform.position.z) * Mathf.Rad2Deg)) < 2f
            ? 0f
            : Mathf.Clamp(Mathf.DeltaAngle(transform.eulerAngles.y, Mathf.Atan2(startPosition.x - transform.position.x, startPosition.z - transform.position.z) * Mathf.Rad2Deg) / 90f, -1f, 1f);

        // Fly to home and hold alt
        float horizontalDistance = Vector2.Distance(new Vector2(startPosition.x, startPosition.z), new Vector2(transform.position.x, transform.position.z));
        leftStick.y = Mathf.Clamp(((horizontalDistance > 1.0f ? 50f : 1f) - distanceFromGround) / 3f, -1f, 1f);
        rightStick.y = Mathf.Clamp(horizontalDistance / 10, 0f, 1f);

        // End condition
        rthTriggered = horizontalDistance > 0.5f || distanceFromGround > 1.5f;
    }

    void FlightPause()
    {
        flightPause = !flightPause;
        mode = Mode.Normal;
    }
}