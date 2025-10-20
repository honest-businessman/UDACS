using UnityEngine;

public class TutorialCanvasManager : MonoBehaviour
{
    public Canvas remoteCanvas;
    public Canvas simulatorCanvas;

    public void ShowSimulator()
    {
        remoteCanvas.gameObject.SetActive(false);
        simulatorCanvas.gameObject.SetActive(true);
    }

    public void ShowRemote()
    {
        simulatorCanvas.gameObject.SetActive(false);
        remoteCanvas.gameObject.SetActive(true);
    }
}

