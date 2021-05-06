using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField]
    private AudioMixer audioMixer;

    [SerializeField] private string masterVolumeParam = "MasterVolume";
    [SerializeField] private string musicVolumeParam = "MusicVolume";
    [SerializeField] private string soundsVolumeParam = "SoundsVolume";

    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider soundsVolumeSlider;

    private const string musicVolumeKey = "MusicVolume";
     private const string soundsVolumeKey = "SoundsVolume";
     private const float minVol = 0.0001f;
     
     
     public float MusicVol => PlayerPrefs.GetFloat(musicVolumeKey, 1f);
     public float SoundsVol => PlayerPrefs.GetFloat(soundsVolumeKey, 1f);

     private void Start()
     {
         SetMusicVolume(MusicVol);
         SetSoundsVolume(SoundsVol);
         UpdateSlider();
     }

    public void SetMusicVolume(float sliderValue)
    {
        audioMixer.SetFloat(musicVolumeParam, Mathf.Log10(sliderValue) * 20);
    }
    public void SetSoundsVolume(float sliderValue)
    {
        audioMixer.SetFloat(soundsVolumeParam, Mathf.Log10(sliderValue) * 20);
    }
    public void SetMasterVolume(float sliderValue)
    {
        audioMixer.SetFloat(masterVolumeParam, Mathf.Log10(sliderValue) * 20);
    }

    private void SetVolume(string param, float volumeValue)
    {
        var mixerVolume = volumeValue<= minVol ? -80 : Mathf.Log10(volumeValue) * 20;
        audioMixer.SetFloat(param, mixerVolume);
    }

    private static void SaveMusicVolume(float vol)
    {
        PlayerPrefs.SetFloat(musicVolumeKey, vol);
    }
    private static void SaveSoundsVolume(float vol)
    {
        PlayerPrefs.SetFloat(soundsVolumeKey, vol);
    }

    private void UpdateSlider()
    {
        musicVolumeSlider.SetValueWithoutNotify(MusicVol);
        soundsVolumeSlider.SetValueWithoutNotify(SoundsVol);
    }
}
