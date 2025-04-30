using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static DialogueData;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    private string _brunchesName;
    private List<Dialogue> _currentDialogues;
    private List<DialogueLine> _currentDialogueLines;
    private DialogueLine _currentDialogueLine;
    private int _currentDialogueLineIndex;
    private int _currentSentenceIndex;

    public UnityEvent onDialogueStarted;
    public UnityEvent<DialogueLine, int> onDialogueLineActive;
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
        onDialogueLineActive?.Invoke(dialogueLine, _currentSentenceIndex);
    }

    public void NextDialogueLine()
    {
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