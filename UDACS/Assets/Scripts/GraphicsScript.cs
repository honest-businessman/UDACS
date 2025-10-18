using UnityEngine;
public class GraphicsScript : MonoBehaviour
{
    void Start()
    {
        Application.targetFrameRate = 60;
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
    }
    public void UpdateFpsLock(int fps) => Application.targetFrameRate = fps;
    public void ChangeScreenMode() => Screen.fullScreenMode = (FullScreenMode)(((int)Screen.fullScreenMode + 1) % 4);
}
