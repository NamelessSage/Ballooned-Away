using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MixerController : MonoBehaviour
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
     
     
     public static float MusicVol => PlayerPrefs.GetFloat(musicVolumeKey, 1f);
     public static float SoundsVol => PlayerPrefs.GetFloat(soundsVolumeKey, 1f);

     private void Start()
     {
         SetMusicVolume(MusicVol);
         SetSoundsVolume(SoundsVol);
         UpdateSliders();
     }

    public void SetMusicVolume(float sliderValue)
    {
        SetVolume(musicVolumeParam, sliderValue);
        SaveMusicVolume(sliderValue);
    }
    public void SetSoundsVolume(float sliderValue)
    {
        SetVolume(soundsVolumeParam, sliderValue);
        SaveSoundsVolume(sliderValue);
    }
    
    public void SetMasterVolume(float sliderValue)
    {
        SetVolume(masterVolumeParam, sliderValue);
    }

    private void SetVolume(string param, float volumeValue)
    {
        var mixerVolume = (volumeValue <= minVol) ? -80 : Mathf.Log(volumeValue) * 20;
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
    
    private void UpdateSliders()
    {
        musicVolumeSlider.SetValueWithoutNotify(MusicVol);
        soundsVolumeSlider.SetValueWithoutNotify(SoundsVol);
    }

    
}
