using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;
    
    [SerializeField] private GameObject pauseMenuUI = null;
    
    private InputMaster _input;
    // private bool gameWasSaved = false;    // TODO

    private void Awake()
    {
        _input = new InputMaster();
        _input.Gameplay.OpenMenu.performed += _ => PauseResumeGame();
        pauseMenuUI.SetActive(false);
    }

    public void PauseResumeGame()
    {
        if (gameIsPaused)
        {
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            gameIsPaused = false;
        } 
        else
        {
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            gameIsPaused = true;
        }
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        gameIsPaused = false;
        SceneManager.LoadScene(0);
    }

    public void TempButton()
    {
        Debug.Log("This button will have its functionality added soon!");
    }

    // Enables and disables Input Actions when this script is instantiated
    private void OnEnable()
    {
        _input.Gameplay.Enable();
        _input.Player.Enable();
    }

    private void OnDisable()
    {
        _input.Gameplay.Disable();
        _input.Player.Disable();
    }
}
