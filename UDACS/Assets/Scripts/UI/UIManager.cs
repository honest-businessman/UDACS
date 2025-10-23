using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UIManager: MonoBehaviour
{
    public bool isGameScene;

    [Header("Required")]
    public Toggle pauseWhilePausedToggle;
    public Toggle infiniteAmmoToggle;
    public Toggle invertRightStickToggle;
    public Toggle dayCycleToggle;
    public Slider snapCameraAngleSlider;
    public Slider deadzoneSlider;
    public Slider volumeSlider;

    DroneController droneController;
    ThrowObject payloadSystem;

    [Header("Game Scene Only")]
    public PausePlay pausePlayScript;
    public DayNight dayNightScript;
    public GameObject droneObject;
    public ThrowObject loadingSystem;
    public GameObject grenadeUI;
    void Start()
    {
        if (isGameScene)
        {
            droneController = droneObject.GetComponent<DroneController>();
            payloadSystem = droneController.GetComponent<ThrowObject>();
        }
        SetToggles();
        SetSettings();
    }
    void SetToggles()
    {
        pauseWhilePausedToggle.isOn = Settings.PauseWhilePaused;
        infiniteAmmoToggle.isOn = Settings.InfiniteAmmo;
        invertRightStickToggle.isOn = Settings.InvertRightStick;
        dayCycleToggle.isOn = Settings.DayCycle;

        snapCameraAngleSlider.value = Settings.SnapCameraAngle;
        deadzoneSlider.value = Settings.Deadzone;
        volumeSlider.value = Settings.Volume;
    }
    void SetSettings()
    {
        if (isGameScene)
        {
            pausePlayScript.pauseWhenOpen = Settings.PauseWhilePaused;
            pausePlayScript.SetTimeScale();
            payloadSystem.requiresLoading = Settings.InfiniteAmmo;
            droneController.invertRightStick = Settings.InvertRightStick;
            dayNightScript.dayLength = Settings.DayCycle ? DayLength.Overdrive : DayLength.None;

            droneController.upwardCameraPosition = Settings.SnapCameraAngle;
            droneController.deadzone = Settings.Deadzone;
            Audio.instance.mixer.SetFloat("MasterVolume", Settings.Volume - 80f);
        }

        snapCameraAngleSlider.GetComponentsInChildren<Text>()[isGameScene ? 1 : 0].text = $"{Settings.SnapCameraAngle:0}";
        deadzoneSlider.GetComponentsInChildren<Text>()[isGameScene ? 1 : 0].text = $"{Settings.Deadzone:0.0}";
        volumeSlider.GetComponentsInChildren<Text>()[isGameScene ? 1 : 0].text = $"{Settings.Volume:0}";
    }
    void Update()
    {
        if (isGameScene) grenadeUI.SetActive(loadingSystem.loaded);
    }
    public void ExitGame() => Application.Quit();
    public void Respawn()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("TerrainScene");
    }
    public void MainMenu() => SceneManager.LoadScene("Main Menu Scene");
    public void PauseWhilePaused(Toggle toggle)
    {
        Settings.PauseWhilePaused = toggle.isOn;
        if (isGameScene)
        {
            pausePlayScript.pauseWhenOpen = toggle.isOn;
            pausePlayScript.SetTimeScale();
        }
    }
    public void InfiniteAmmo(Toggle toggle)
    {
        Settings.InfiniteAmmo = toggle.isOn;
        if (isGameScene)
        {
            payloadSystem.requiresLoading = toggle.isOn;
            if (toggle.isOn) loadingSystem.loaded = true;
        }
    }
    public void InvertRightStick(Toggle toggle)
    {
        Settings.InvertRightStick = toggle.isOn;
        if (isGameScene) droneController.invertRightStick = toggle.isOn;
    }
    public void DayCycle(Toggle toggle)
    {
        Settings.DayCycle = toggle.isOn;
        if (isGameScene) dayNightScript.dayLength = toggle.isOn ? DayLength.Overdrive : DayLength.None;
    }
    public void SnapCameraAngle(Slider slider)
    {
        Settings.SnapCameraAngle = (byte)slider.value;
        if (isGameScene) droneController.upwardCameraPosition = slider.value;
        slider.GetComponentsInChildren<Text>()[isGameScene ? 1 : 0].text = $"{slider.value:0}";
    }
    public void Deadzone(Slider slider)
    {
        Settings.Deadzone = (byte)slider.value;
        if (isGameScene) droneController.deadzone = slider.value;
        slider.GetComponentsInChildren<Text>()[isGameScene ? 1 : 0].text = $"{slider.value:0.0}";

    }
    public void Volume(Slider slider)
    {
        Settings.Volume = (byte)slider.value;
        if (isGameScene) Audio.instance.mixer.SetFloat("MasterVolume", slider.value - 80f);
        slider.GetComponentsInChildren<Text>()[isGameScene ? 1 : 0].text = $"{slider.value:0}";
    }
}
