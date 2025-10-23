using UnityEngine;

public static class Settings
{
    public static bool InfiniteAmmo { get; set; }
    public static bool PauseWhilePaused { get; set; }
    public static bool InvertRightStick { get; set; }
    public static bool DayCycle { get; set; }
    public static byte SnapCameraAngle { get; set; }
    public static float Deadzone { get; set; }
    public static byte Volume { get; set; }

    static bool initRun;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        if (!initRun)
        {
            initRun = true;
            InfiniteAmmo = false;
            PauseWhilePaused = true;
            InvertRightStick = false;
            DayCycle = false;
            SnapCameraAngle = 45;
            Deadzone = 0.1f;
            Volume = 80;
        }
    }
}