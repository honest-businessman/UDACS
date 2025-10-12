using UnityEngine;
using UnityEngine.Audio;

public class Audio : MonoBehaviour
{
    public static Audio instance;

    public AudioMixer mixer;
    public AudioSource DroneSource;
    public AudioSource VoiceLineSource;
    public AudioClip[] DroneSounds;
    public AudioClip[] VoiceLines;

    public float Dronepitch;
    public float Voicepitch;

    public void Awake() // This makes sure that there is only one instance of the AudioManager
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void PlayDroneSound() //call this function to play a drone sound
    {
        AudioClip clip = System.Array.Find(DroneSounds, clip => clip.name == "SoundName");
        if (clip == null)
        {   Debug.LogWarning("Sound not found!");   return;}
        DroneSource.pitch = Dronepitch; //this controls the pitch of the drone sounds
        DroneSource.PlayOneShot(clip);
    }
    public void PlayVoiceLine() //call this function to play a voice line
    {
        AudioClip clip = System.Array.Find(VoiceLines, clip => clip.name == "SoundName");
        if (clip == null)
        { Debug.LogWarning("Sound not found!"); return; }
        VoiceLineSource.pitch = Voicepitch; //this controls the pitch of the voice lines
        VoiceLineSource.PlayOneShot(clip);
    }
    public void SetDroneVolume() //call this function to set the volume of the drone sounds
    {
        float volume = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
        mixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }
    public void SetVoiceVolume() //call this function to set the volume of the voice lines
    {
        float volume = PlayerPrefs.GetFloat("VoiceVolume", 0.75f);
        mixer.SetFloat("VoiceVolume", Mathf.Log10(volume) * 20);
    }
}
