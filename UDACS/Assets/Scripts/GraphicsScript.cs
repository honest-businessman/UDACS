using UnityEngine;
public static class GraphicsScript
{
    static bool initRun;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        if (!initRun)
        {
            initRun = true;
            Application.targetFrameRate = 60;
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
    }
    public static void UpdateFpsLock(int fps) => Application.targetFrameRate = fps;
    public static void ChangeScreenMode() => Screen.fullScreenMode = (FullScreenMode)(((int)Screen.fullScreenMode + 1) % 4);
}