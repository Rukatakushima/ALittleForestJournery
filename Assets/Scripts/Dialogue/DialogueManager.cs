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
    // public int DialoguesCount => CurrentDialogues.Count;
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

        SetDialogueData(baseDialogueData);
    }

    public void SetDialogueData(DialogueData dialogueData) => CurrentDialogues = dialogueData.dialogues;

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

        bool isValid = dialogue != null && dialogue.DialogueLines?.Count > 0;
        if (isValid)
        {
            currentDialogueLines = dialogue.DialogueLines;
            currentDialogueLineIndex = 0;

            isValid = !(currentDialogueLines[currentDialogueLineIndex].Sentences.Count <= 0 || currentSentenceIndex > currentDialogueLines[currentDialogueLineIndex].Sentences.Count);
        }

        if (!isValid) Debug.LogWarning($"Dialogue {id} not found or empty or Sentence is out of array");

        return isValid;
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

    // public void FastForward()
    // {
    //     if (typingCoroutine == null) return;

    //     StopTyping();
    //     view.SetDialogueText(currentDialogueLines[currentDialogueLineIndex].Text);
    // }
}