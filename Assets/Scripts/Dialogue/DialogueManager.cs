using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static DialogueData;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Configuration")]
    [SerializeField] private List<DialogueData> dialogueDatabases = new();
    private Dictionary<string, DialogueData> dialoguesDictionary = new();

    [Header("Runtime Data")]
    private string brunchesName;
    private List<Dialogue> CurrentDialogues;
    private List<DialogueLine> currentDialogueLines;
    private DialogueLine currentDialogueLine;
    private int currentDialogueLineIndex;
    private int currentSentenceIndex;

    [Header("Events")]
    public UnityEvent OnDialogueStarted;
    public UnityEvent<DialogueLine, int> OnDialogueLineActive;
    public UnityEvent OnDialogueEnded;

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
        dialoguesDictionary.Clear();
        foreach (DialogueData data in dialogueDatabases)
        {
            if (!dialoguesDictionary.ContainsKey(data.DialogueName))
                dialoguesDictionary.Add(data.DialogueName, data);
            else
                Debug.LogWarning($"Duplicate dialogue ID found: {data.DialogueName}");
        }
    }

    public void AddDialogueDatabase(DialogueData database)
    {
        if (!dialogueDatabases.Contains(database))
        {
            dialogueDatabases.Add(database);
            if (!dialoguesDictionary.ContainsKey(database.DialogueName))
                dialoguesDictionary.Add(database.DialogueName, database);
        }
    }

    public void SetCurrentDialogueData(string dialogueName)
    {
        CurrentDialogues = dialoguesDictionary[dialogueName].dialogues;
        brunchesName = dialogueName;
        Debug.Log("Setted " + brunchesName);
    }

    public void StartDialogue(int dialogueID)
    {
        if (!TryGetDialogue(dialogueID, out Dialogue dialogue)) return;

        StartDialogueLine(currentDialogueLines[currentDialogueLineIndex]);
        OnDialogueStarted?.Invoke();

        dialogue.IsRead = true;
    }

    private bool TryGetDialogue(int id, out Dialogue dialogue)
    {
        dialogue = CurrentDialogues.Find(d => d.ID == id);

        if (dialogue != null && dialogue.DialogueLines?.Count > 0)
        {
            currentDialogueLines = dialogue.DialogueLines;
            currentDialogueLineIndex = 0;

            return !(currentDialogueLines[currentDialogueLineIndex].Sentences.Count <= 0
            || currentSentenceIndex > currentDialogueLines[currentDialogueLineIndex].Sentences.Count);
        }
        else
        {
            Debug.LogWarning($"Dialogue {id} not found or empty or Sentence is out of array");
            return false;
        }
    }

    private void StartDialogueLine(DialogueLine dialogueLine)
    {
        if (dialogueLine == null) return;

        currentDialogueLine = dialogueLine;
        currentSentenceIndex = 0;
        OnDialogueLineActive?.Invoke(dialogueLine, currentSentenceIndex);
    }

    public void NextDialogueLine()
    {
        currentSentenceIndex++;

        if (currentDialogueLine == null ||
            currentSentenceIndex >= currentDialogueLine.Sentences.Count)
        {
            if (++currentDialogueLineIndex < currentDialogueLines.Count)
            {
                currentSentenceIndex = 0;
                StartDialogueLine(currentDialogueLines[currentDialogueLineIndex]);
            }
            else
            {
                EndDialogue();
            }
        }
        else
        {
            OnDialogueLineActive?.Invoke(currentDialogueLine, currentSentenceIndex);
        }
    }

    public void EndDialogue()
    {
        currentDialogueLines = null;
        currentDialogueLineIndex = 0;
        OnDialogueEnded?.Invoke();
    }
}