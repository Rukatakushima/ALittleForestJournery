using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using static DialogueData;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Configuration")]
    [SerializeField] private List<DialogueData> dialogueDatabases = new();
    private readonly Dictionary<string, DialogueData> _dialoguesDictionary = new();

    [Header("Runtime Data")]
    private string _brunchesName;
    private List<Dialogue> _currentDialogues;
    private List<DialogueLine> _currentDialogueLines;
    private DialogueLine _currentDialogueLine;
    private int _currentDialogueLineIndex;
    private int _currentSentenceIndex;

    [Header("Events")]
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

        InitializeDialoguesDictionary();
    }

    private void InitializeDialoguesDictionary()
    {
        _dialoguesDictionary.Clear();
        foreach (var data in dialogueDatabases.Where(data => !_dialoguesDictionary.TryAdd(data.DialogueName, data)))
        {
            Debug.LogWarning($"Duplicate dialogue ID found: {data.DialogueName}");
        }
    }

    public void AddDialogueDatabase(DialogueData database)
    {
        if (dialogueDatabases.Contains(database)) return;
        dialogueDatabases.Add(database);
        _dialoguesDictionary.TryAdd(database.DialogueName, database);
    }

    public void SetCurrentDialogueData(string dialogueName)
    {
        _currentDialogues = _dialoguesDictionary[dialogueName].dialogues;
        _brunchesName = dialogueName;
        Debug.Log("Set " + _brunchesName);
    }

    public void StartDialogue(int dialogueID)
    {
        if (!TryGetDialogue(dialogueID, out Dialogue dialogue)) return;

        StartDialogueLine(_currentDialogueLines[_currentDialogueLineIndex]);
        onDialogueStarted?.Invoke();

        dialogue.isRead = true;
    }

    private bool TryGetDialogue(int id, out Dialogue dialogue)
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

        if (_currentDialogueLine == null || _currentSentenceIndex >= _currentDialogueLine.sentences.Count)
        {
            if (_currentDialogueLines == null)
            {
                Debug.LogWarning("Current Dialogue Lines are null");
                EndDialogue();
            }
            // Debug.Log(currentDialogueLines.Count);
            else if (/*currentDialogueLines != null &&*/ ++_currentDialogueLineIndex < _currentDialogueLines.Count)
            {
                _currentSentenceIndex = 0;
                StartDialogueLine(_currentDialogueLines[_currentDialogueLineIndex]);
            }
            else
            {
                EndDialogue();
            }
        }
        else
        {
            onDialogueLineActive?.Invoke(_currentDialogueLine, _currentSentenceIndex);
        }
    }

    public void EndDialogue()
    {
        _currentDialogueLines = null;
        _currentDialogueLineIndex = 0;
        onDialogueEnded?.Invoke();
    }
}