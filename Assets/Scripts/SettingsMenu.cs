using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public TMP_Dropdown resolutionDropdown;
    Resolution[] resolutions;

    void Start()
    {
        #if UNITY_WEBGL
        resolutionDropdown.gameObject.SetActive(false);
        #endif

        float volume;
        audioMixer.GetFloat("Volume", out volume);
        GameObject.Find("Volume Slider").GetComponent<UnityEngine.UI.Slider>().value = volume;

        int quality = QualitySettings.GetQualityLevel();
        GameObject.Find("Graphics Dropdown").GetComponent<TMP_Dropdown>().value = quality;

        bool fullscreen = Screen.fullScreen;
        GameObject.Find("Fullscreen Toggle").GetComponent<UnityEngine.UI.Toggle>().isOn = fullscreen;

        #if !UNITY_WEBGL
        Resolution[] resolutions = Screen.resolutions;
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        if (resolutions.Length == 0)
        {
            resolutionDropdown.ClearOptions();
            options.Add("1280 x 720");
            resolutionDropdown.AddOptions(options);
            currentResolutionIndex = 0;
            resolutionDropdown.RefreshShownValue();
        }
        else
        {
            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }
        #endif

    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = Screen.resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

}
