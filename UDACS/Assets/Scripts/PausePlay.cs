using UnityEngine;
public class PausePlay : MonoBehaviour
{
    public bool Paused = false;
    public Canvas pauseCanvas;
    void Update()
    {
        if (PlayerInteraction.Pause.triggered)
        {
            Paused = !Paused;
            pauseCanvas.gameObject.SetActive(Paused);
            Time.timeScale = Paused ? 0 : 1;
        }
    }
}