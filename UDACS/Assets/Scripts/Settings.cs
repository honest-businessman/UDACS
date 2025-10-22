public static class Settings
{
    public static bool InfiniteAmmo { get; set; }
    public static bool PauseWhilePaused { get; set; }
    public static bool InvertRightStick { get; set; }
    public static bool DayCycle { get; set; }
    public static byte SnapCameraAngle { get; set; }
    public static float Deadzone { get; set; }
    public static byte Volume { get; set; }
    static Settings()
    {
        InfiniteAmmo = false;
        PauseWhilePaused = true;
        InvertRightStick = false;
        DayCycle = false;
        SnapCameraAngle = 45;
        Deadzone = 0.1f;
        Volume = 80;
    }
}