using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static DialogueData;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    // private string _brunchesName;
    private List<Dialogue> _currentDialogues;
    private List<DialogueLine> _currentDialogueLines;
    private DialogueLine _currentDialogueLine;
    private int _currentDialogueLineIndex;
    private int _currentSentenceIndex;
    
    private List<ChoiceLine> _currentChoiceLines;
    private int _currentChoiceIndex;

    public UnityEvent onDialogueStarted;
    public UnityEvent<DialogueLine, int> onDialogueLineActive;
    public UnityEvent<DialogueLine, int> onChoiceLineActive;
    public UnityEvent<string> onChoiceMade;
    public UnityEvent onDialogueEnded;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetCurrentDialogueData(List<Dialogue> dialogues) => _currentDialogues = dialogues;

    public void StartDialogue(string dialogueID)
    {
        if (!TryGetDialogue(dialogueID, out Dialogue dialogue)) return;
        
        CacheChoices(dialogue);

        StartDialogueLine(_currentDialogueLines[_currentDialogueLineIndex]);
        onDialogueStarted?.Invoke();
        
        dialogue.isRead = true;
    }

    private bool TryGetDialogue(string id, out Dialogue dialogue)
    {
        dialogue = _currentDialogues.Find(d => d.ID == id);

        if (dialogue is { DialogueLines: { Count: > 0 } })
        {
            _currentDialogueLines = dialogue.DialogueLines;
            _currentDialogueLineIndex = 0;

            return !(_currentDialogueLines[_currentDialogueLineIndex].sentences.Count <= 0
            || _currentSentenceIndex > _currentDialogueLines[_currentDialogueLineIndex].sentences.Count);
        }
        else
        {
            EndDialogue();
            Debug.LogWarning($"Dialogue {id} not found or empty or Sentence is out of array");
            return false;
        }
    }

    private void CacheChoices(Dialogue dialogue)
    {
        _currentChoiceLines = new List<ChoiceLine>();
        foreach (var line in dialogue.DialogueLines)
        {
            if (line is ChoiceLine choiceLine)
                _currentChoiceLines.Add(choiceLine);
        }
        _currentChoiceIndex = 0;
    }

    private void StartDialogueLine(DialogueLine dialogueLine)
    {
        if (dialogueLine == null)
        {
            EndDialogue();
            Debug.LogWarning("DialogueLine is null ");
            return;
        }

        _currentDialogueLine = dialogueLine;
        _currentSentenceIndex = 0;

        if (dialogueLine is ChoiceLine choiceLine)
        {
            onChoiceLineActive?.Invoke(choiceLine, _currentSentenceIndex);
        }
        else
        {
            onDialogueLineActive?.Invoke(dialogueLine, _currentSentenceIndex);
        }
    }
    
    public void ShowNextChoiceLine()
    {
        if (++_currentChoiceIndex >= _currentChoiceLines.Count) return;

        StartDialogueLine(_currentChoiceLines[_currentChoiceIndex]);
    }

    public void ShowPreviousChoiceLine()
    {
        if (--_currentChoiceIndex < 0) return;
        
        StartDialogueLine(_currentChoiceLines[_currentChoiceIndex]);
    }


    public void NextDialogueLine()
    {
        if (_currentDialogueLine is ChoiceLine choiceLine)
        {
            StartDialogue(choiceLine.nextDialogueID);
            onChoiceMade?.Invoke(choiceLine.nextDialogueID);
            return;
        }

        _currentSentenceIndex++;

        if (_currentDialogueLine != null && _currentSentenceIndex < _currentDialogueLine.sentences.Count)
        {
            // Next Sentence
            onDialogueLineActive?.Invoke(_currentDialogueLine, _currentSentenceIndex);
        }
        else if (_currentDialogueLine == null || ++_currentDialogueLineIndex >= _currentDialogueLines.Count)
        {
            EndDialogue();
        }
        else
        {
            // Next Dialogue Line
            _currentSentenceIndex = 0;
            StartDialogueLine(_currentDialogueLines[_currentDialogueLineIndex]);
        }
    }

    public void EndDialogue()
    {
        _currentDialogueLines = null;
        _currentDialogueLineIndex = 0;
        onDialogueEnded?.Invoke();
    }
}