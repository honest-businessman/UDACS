using UnityEngine;
public class PausePlay : MonoBehaviour
{
    public bool Paused = false;
    public Canvas pauseCanvas;
    public bool pauseWhenOpen = true;

    void Start()
    {
        pauseCanvas.gameObject.SetActive(Paused);
    }
    void Update()
    {
        if (PlayerInteraction.Pause.triggered)
        {
            Paused = !Paused;
            pauseCanvas.gameObject.SetActive(Paused);
            if (pauseWhenOpen) Time.timeScale = Paused ? 0 : 1;
        }
    }
}
