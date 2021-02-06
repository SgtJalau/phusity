using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;    // Allows calling from other scripts

    [SerializeField] private List<GameObject> menus = new List<GameObject>();
    [SerializeField] private List<TextMeshProUGUI> percentages = new List<TextMeshProUGUI>();
    [SerializeField] private TMPro.TMP_Dropdown resDropdown = null;

    public AudioMixer audioMixer;
    public List<GameObject> rebindButton = new List<GameObject>();

    private Resolution[] resolutions;
    private InputDevice[] devices;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;    // Allows calling from other scripts
        }
        else
        {
            Destroy(gameObject); // If two ore more MainMenu instances should ever be present, then this will delete all but one of them
                                 // This is a fallback and should never be needed if possible
        }
    }

    private void Start()
    {
        // This will set the MainMenu object to the only active menu on game startup 
        menus[0].SetActive(true);

        for (int i = 1; i < menus.Count; i++)
        {
            menus[i].SetActive(false);
        }

        // This will find all display resolutions currently available and configure the resolution dropdown in the Resolution Menu accordingly
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

        // This will find all input devices currently available and configure the resolution dropdown in the Controls Menu accordingly
        // TODO
    }

    // Starts the game without looking for save files (currently)
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

    // Changes the device to whatever was set in the resolutions dropdown
    public void SetDevice(int resIndex)
    {
        // TODO
    }

    // Sets sound volumes for music and sound effects as well as a master level
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