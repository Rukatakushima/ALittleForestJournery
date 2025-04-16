using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static DialogueData;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Configuration")]
    [SerializeField] private DialogueData baseDialogueData;
    private List<Dialogue> CurrentDialogues;
    public int DialoguesCount => CurrentDialogues.Count;
    private List<DialogueLine> currentDialogueLines;
    private int currentDialogueLineIndex;

    [Header("Events")]
    public UnityEvent OnDialogueStarted;
    public UnityEvent<DialogueLine> OnDialogueLineActive;
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

        SetDialogueData(baseDialogueData);
    }

    public void SetDialogueData(DialogueData dialogueData) => CurrentDialogues = dialogueData.dialogues;

    public void StartDialogue(int dialogueID)
    {
        if (!TryGetDialogue(dialogueID, out Dialogue dialogue)) return;

        PrepareDialogue(dialogue);
        OnDialogueStarted?.Invoke();
        StartDialogueLine(currentDialogueLines[0]);
    }

    private bool TryGetDialogue(int id, out Dialogue dialogue)
    {
        dialogue = CurrentDialogues.Find(d => d.ID == id);
        bool isValid = dialogue != null && dialogue.DialogueLines?.Count > 0;
        if (!isValid) Debug.LogWarning($"Dialogue {id} not found or empty");
        return isValid;
    }

    private void PrepareDialogue(Dialogue dialogue)
    {
        currentDialogueLines = dialogue.DialogueLines;
        currentDialogueLineIndex = 0;
        dialogue.IsRead = true;
    }

    private void StartDialogueLine(DialogueLine dialogueLine)
    {
        if (dialogueLine == null) return;

        OnDialogueLineActive?.Invoke(dialogueLine);
    }

    public void NextDialogueLine()
    {
        if (++currentDialogueLineIndex < currentDialogueLines.Count)
            StartDialogueLine(currentDialogueLines[currentDialogueLineIndex]);
        else
            EndDialogue();
    }

    public void EndDialogue()
    {
        currentDialogueLines = null;
        currentDialogueLineIndex = 0;
        OnDialogueEnded?.Invoke();
    }

    // public void FastForward()
    // {
    //     if (typingCoroutine == null) return;

    //     StopTyping();
    //     view.SetDialogueText(currentDialogueLines[currentDialogueLineIndex].Text);
    // }
}