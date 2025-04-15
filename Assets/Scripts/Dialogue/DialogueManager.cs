using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static DialogueData;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Configuration")]
    [SerializeField] private DialogueView view;
    [SerializeField] private DialogueData dialogueData;
    [SerializeField] private DialogueData baseDialogueData;

    private List<Dialogue> CurrentDialogues;
    public int DialoguesCount => CurrentDialogues.Count;

    private List<DialogueLine> currentDialogueLines;
    private int currentDialogueLineIndex;

    private Coroutine typingCoroutine;

    // [Header("Events")]
    // public UnityEvent<DialogueLine> OnDialogueLineActive;
    // public UnityEvent OnDialogueEnded;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (view == null)
            view = GetComponentInChildren<DialogueView>();

        SetDialogueData(baseDialogueData);
    }

    public void SetDialogueData(DialogueData dialogueData)
    {
        this.dialogueData = dialogueData;
        CurrentDialogues = dialogueData.dialogues;
    }

    public void StartDialogue(int dialogueID)
    {
        if (!TryGetDialogue(dialogueID, out Dialogue dialogue)) return;

        PrepareDialogue(dialogue);
        view.ToggleDialogueBox(true);
        StartDialogueLine(currentDialogueLines[0]);
    }

    private bool TryGetDialogue(int id, out Dialogue dialogue)
    {
        dialogue = CurrentDialogues.Find(d => d.ID == id);

        if (dialogue == null)
        {
            Debug.LogWarning($"Dialogue not found");
            return false;
        }

        if (dialogue.DialogueLines == null || !dialogue.DialogueLines.Any())
        {
            Debug.LogWarning($"Dialogue {dialogue.ID} has no Dialogue Lines");
            return false;
        }

        return true;
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

        StopTyping();
        view.UpdateDialogueBox(dialogueLine.Speaker.Name, dialogueLine.Speaker.Transform);
        typingCoroutine = StartCoroutine(TypeSentence(dialogueLine.Text, dialogueLine.Speed));

        // OnDialogueLineActive?.Invoke(currentDialogueLines[currentDialogueLineIndex]);
        dialogueLine.ActiveDialogueLineEvent();
    }

    private void StopTyping()
    {
        if (typingCoroutine == null) return;

        StopCoroutine(typingCoroutine);
        typingCoroutine = null;
    }

    private IEnumerator TypeSentence(string text, float speed)
    {
        var waitTime = new WaitForSeconds(Time.deltaTime * speed);

        for (int i = 0; i < text.Length; i++)
        {
            view.SetDialogueText(text.Substring(0, i + 1));
            yield return waitTime;
        }

        typingCoroutine = null;
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
        StopTyping();
        view.CloseDialogueBox();
        // OnDialogueEnded?.Invoke();
        CurrentDialogues[currentDialogueLineIndex].ActiveEndDialogueEvent();
    }

    public void FastForward()
    {
        if (typingCoroutine == null) return;

        StopTyping();
        view.SetDialogueText(currentDialogueLines[currentDialogueLineIndex].Text);
    }
}