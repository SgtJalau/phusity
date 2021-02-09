using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class KeyRebinding : MonoBehaviour
{
    [SerializeField, Tooltip("This contains all Buttons in the Controls Menu")]
    private List<GameObject> rebindButton = new List<GameObject>();

    [SerializeField, Tooltip("This stores the Input References for each Button in the Controls Menu")] 
    private List<InputActionReference> bindingRef = new List<InputActionReference>();

    [SerializeField, Tooltip("If an Input Reference has an Action that is not a Binding, then this contains the wanted index")] 
    private List<int> index = new List<int>();

    private InputActionRebindingExtensions.RebindingOperation rbOperation = null;
    private TextMeshProUGUI displayText;
    private int bindIndex;

    private void Start()
    {
        string rebinds = PlayerPrefs.GetString("rebinds", string.Empty);

        // Loads a player-specific keybinding from a json file when it has been previously changed
        if (string.IsNullOrEmpty(rebinds) == false)
        {
            bindingRef[0].asset.LoadBindingOverridesFromJson(rebinds);
        }

        for (int i = 0; i < rebindButton.Count; i++)
        {
            // This gets the index of each keybinding, considering the current control scheme and potential composite bindings
            bindIndex = bindingRef[i].action.GetBindingIndexForControl(bindingRef[i].action.controls[0]) + index[i];

            // Loads in the text of each button
            displayText = rebindButton[i].GetComponentInChildren<TextMeshProUGUI>();
            displayText.text = InputControlPath.ToHumanReadableString(bindingRef[i].action.bindings[bindIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        }
    }

    // This starts the rebinding process and also changes some other parameters
    public void StartRebinding(int buttonIndex)
    {
        for (int i = 0; i < rebindButton.Count; i++)
        {
            // Disable all buttons but the one that called this function
            if (buttonIndex != i)
            {
                rebindButton[i].GetComponent<Button>().interactable = false;
            }
            // Change the text of the Button that called this function
            else
            {
                displayText = rebindButton[buttonIndex].GetComponentInChildren<TextMeshProUGUI>();
                displayText.text = "...";
            }
        }

        bindIndex = bindingRef[buttonIndex].action.GetBindingIndexForControl(bindingRef[buttonIndex].action.controls[0]) + index[buttonIndex];

        // This does the rebinding operation
        rbOperation = bindingRef[buttonIndex].action.PerformInteractiveRebinding(bindIndex)
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => RebindComplete(buttonIndex))
            .WithCancelingThrough("<Keyboard>/escape")
            .OnCancel(operation => RebindComplete(buttonIndex))
            .Start();
    }

    // This disposes some data and also updates some parameters
    private void RebindComplete(int buttonIndex)
    {
        bindIndex = bindingRef[buttonIndex].action.GetBindingIndexForControl(bindingRef[buttonIndex].action.controls[0]) + index[buttonIndex];
        displayText.text = InputControlPath.ToHumanReadableString(bindingRef[buttonIndex].action.bindings[bindIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        
        // Disposes rebind operation data to prevent memory leakage
        rbOperation.Dispose();

        // Re-enables all buttons again
        for  (int i = 0; i < rebindButton.Count; i++)
        {
            rebindButton[i].GetComponentInChildren<Button>().interactable = true;
        }
    }

    // Saves changed keybindings
    public void SaveRebind()
    {
        string rebinds = bindingRef[0].asset.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);
    }
}
