using System;
using TMPro;
using UnityEngine;

public class PlayerGUI : MonoBehaviour
{
    private GameObject _canvas;

    private GameObject _dialogueBox;
    private CanvasGroup _canvasGroup;
    
    private PlayerObject _playerObject;

    //Text content
    private TextMeshProUGUI _dialogueHeadline;
    private TextMeshProUGUI _dialogueText;

    private void Awake()
    {
        _canvas = GameObject.Find("Canvas");
        _dialogueBox = GameObject.Find("DialogueBox");
        GameObject.Find("DialogueName").TryGetComponent(out _dialogueHeadline);
        GameObject.Find("DialogueText").TryGetComponent(out _dialogueText);

        _canvasGroup = _dialogueBox.GetComponent<CanvasGroup>();
        _playerObject = GetComponent<PlayerObject>();
    }


    //Coin Gui
    void OnGUI() 
    {
        GUI.Label(new Rect(10, 10, 100, 20), "Score : " + _playerObject.Points);
        
        GUI.Label(new Rect(10, 25, 200, 20), "Active Ability : " + _playerObject.GetActiveAbility());
    }

    public void SetTextbox(String headline, String body)
    {
        _dialogueHeadline.text = headline;
        _dialogueText.text = body;
    }

    public void HideTextbox()
    {
        _canvasGroup.alpha = 0;
    }

    public void ShowTextbox()
    {
        _canvasGroup.alpha = 1;
    }
}