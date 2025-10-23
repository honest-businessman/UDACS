using UnityEngine;
public class PausePlay : MonoBehaviour
{
    public bool Paused = false;
    public Canvas pauseCanvas;
    public bool pauseWhenOpen = true;

    void Start() => pauseCanvas.gameObject.SetActive(Paused);
    void Update()
    {
        if (PlayerInteraction.Pause.triggered) ChangePauseState();
    }
    public void ChangePauseState()
    {
        Paused = !Paused;
        pauseCanvas.gameObject.SetActive(Paused);
        SetTimeScale();
    }
    public void SetTimeScale() => Time.timeScale = pauseWhenOpen ? Paused ? 0 : 1 : 1;
}
