using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class KeyRebinding : MonoBehaviour
{
    [SerializeField] private InputActionReference keybind = null;
    [SerializeField] private int index = 0;

    private InputActionRebindingExtensions.RebindingOperation rbOperation = null;
    private TextMeshProUGUI displayText;
    private int bindIndex;

    private void Start()
    {
        string rebinds = PlayerPrefs.GetString("rebinds", string.Empty);

        // Loads a player-specific keybinding from a json file when it has been previously changed
        if (string.IsNullOrEmpty(rebinds) == false)
        {
            keybind.asset.LoadBindingOverridesFromJson(rebinds);
        }
        
        // This gets the index for this keybinding, considering the current control scheme and potential composite bindings
        bindIndex = keybind.action.GetBindingIndexForControl(keybind.action.controls[0]) + index;

        // Updates the button text
        displayText = gameObject.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
        displayText.text = InputControlPath.ToHumanReadableString(keybind.action.bindings[bindIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
    }

    // This starts the rebinding process and also changes some other parameters
    public void StartRebinding()
    {
        displayText.text = "...";
        
        // Disable all other buttons
        foreach (GameObject obj in MainMenu.instance.rebindButton)
        {
            if (obj.name != gameObject.transform.parent.name)
            {
                obj.GetComponentInChildren<Button>().interactable = false;
            }
        }

        // This does the rebinding operation
        rbOperation = keybind.action.PerformInteractiveRebinding(bindIndex)
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => RebindComplete())
            .WithCancelingThrough("<Keyboard>/escape")
            .OnCancel(operation => RebindComplete())
            .Start();
    }

    // This disposes some data and also updates some parameters
    private void RebindComplete()
    {
        displayText.text = InputControlPath.ToHumanReadableString(keybind.action.bindings[bindIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        
        // Disposes rebind operation data to prevent memory leakage
        rbOperation.Dispose();

        // Re-enables all buttons again
        foreach (GameObject obj in MainMenu.instance.rebindButton)
        {
            obj.GetComponentInChildren<Button>().interactable = true;
        }

        SaveRebind();
    }

    // Saves a changed keybinding
    public void SaveRebind()
    {
        string rebinds = keybind.asset.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);
    }
}
