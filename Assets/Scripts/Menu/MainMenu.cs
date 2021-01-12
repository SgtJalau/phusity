using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    // public static MainMenu instance;    // Allows calling from another script - This will be useful later for e.g. calling the main menu from in-game

    public List<GameObject> menus = new List<GameObject>();
    public List<TextMeshProUGUI> percentages = new List<TextMeshProUGUI>();
    public AudioMixer audioMixer;
    public TMPro.TMP_Dropdown resDropdown;
    public Resolution[] resolutions;

    private void Start()
    {
        // This will find all display resolutions currently available and configure the resolution dropdown in the options menu accordingly
        resolutions = Screen.resolutions;
        List<string> options = new List<string>();
        int currentRes = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentRes = i;
            }
        }

        resDropdown.ClearOptions();
        resDropdown.AddOptions(options);
        resDropdown.value = currentRes;
        resDropdown.RefreshShownValue();


        // This will set the MainMenu object to the only active menu on game startup 
        menus[0].SetActive(true);

        for (int i = 1; i < menus.Count; i++)
        {
            menus[i].SetActive(false);
        }
    }

    // Starts the without looking for save files
    public void PlayGame () {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Closes the game
    public void QuitGame () {
        Debug.Log("QUIT!");    // This won't do anything in the Unity Game Window, so this message is send as an indicator
        Application.Quit();
    }

    // Sets game to fullscreen mode
    public void SetFullscreen (bool isFull)
    {
        Screen.fullScreen = isFull;
    }

    // Changes the display resolution to whatever was set in the resolutions dropdown
    public void SetResolution(int resIndex)
    {
        Resolution resolution = resolutions[resIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    // Sets sound volumes for music, sound effects and a master level
    public void SetMasterVolume(float volume)
    {
        percentages[0].text = volume + "%";
        if (volume == 0.0f) volume = 0.0001f;
        audioMixer.SetFloat("master", Mathf.Log10(volume / 100.0f) * 20);
    }

    public void SetMusicVolume(float volume)
    {
        percentages[1].text = volume + "%";
        if (volume == 0.0f) volume = 0.0001f;
        audioMixer.SetFloat("music", Mathf.Log10(volume / 100.0f) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        percentages[2].text = volume + "%";
        if (volume == 0.0f) volume = 0.0001f;
        audioMixer.SetFloat("sfx", Mathf.Log10(volume / 100.0f) * 20);
    }
}