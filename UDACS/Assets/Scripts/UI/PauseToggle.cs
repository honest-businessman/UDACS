using UnityEngine;

public class PauseToggle : MonoBehaviour
{
    public GameObject PausePanel;
    public KeyCode key = KeyCode.Escape;
    public bool pauseWhenOpen = true;

    bool isOpen;

    void Start()
    {
        if (PausePanel) PausePanel.SetActive(false);
        ApplyState(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            isOpen = !isOpen;
            if (PausePanel) PausePanel.SetActive(isOpen);

            ApplyState(isOpen);
        }


        if (isOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            isOpen = false;
            if (PausePanel) PausePanel.SetActive(false);
            ApplyState(false);
        }
    }
    void ApplyState(bool open)
    {
        if (pauseWhenOpen) Time.timeScale = open ? 0f : 1f;
    }
}
