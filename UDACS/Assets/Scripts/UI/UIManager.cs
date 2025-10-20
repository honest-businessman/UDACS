using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UIManager: MonoBehaviour
{
    public PausePlay pausePlayScript;
    public DayNight dayNightScript;
    public GameObject droneObject;
    public ThrowObject loadingSystem;
    public GameObject grenadeUI;

    DroneController droneController;
    ThrowObject payloadSystem;
    void Start()
    {
        droneController = droneObject.GetComponent<DroneController>();
        payloadSystem = droneController.GetComponent<ThrowObject>();
    }
    void Update() => grenadeUI.SetActive(loadingSystem.loaded);
    public void ExitGame() => Application.Quit();
    public void Respawn()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("TerrainScene");
    }
    public void MainMenu() => SceneManager.LoadScene("Main Menu");
    public void PauseWhilePaused(Toggle toggle) => pausePlayScript.pauseWhenOpen = toggle.isOn;
    public void InfiniteAmmo(Toggle toggle) => payloadSystem.requiresLoading = toggle.isOn;
    public void InvertRightStick(Toggle toggle) => droneController.invertRightStick = toggle.isOn;
    public void DayCycle(Toggle toggle) => dayNightScript.dayLength = toggle.isOn ? DayLength.Overdrive : DayLength.None;
    public void SnapCameraAngle(Slider slider)
    {
        droneController.upwardCameraPosition = slider.value;
        slider.GetComponentsInChildren<Text>()[1].text = $"{slider.value:0}";
    }
    public void Deadzone(Slider slider)
    {
        droneController.deadzone = slider.value;
        slider.GetComponentsInChildren<Text>()[1].text = $"{slider.value:0.0}";

    }
    public void Volume(Slider slider)
    {
        Audio.instance.mixer.SetFloat("MasterVolume", slider.value - 80f);
        slider.GetComponentsInChildren<Text>()[1].text = $"{slider.value:0}";
    }
}
