using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static DialogueData;
using static DialogueSpeakers;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Configuration")]
    [SerializeField] private DialogueView view;

    // [SerializeField] private DialogueSpeakers Characters;
    // [SerializeField] private TypingSpeeds TypingSpeeds;

    //     [System.Serializable]
    //     public class Sentence
    //     {
    //         [ValueDropdown("GetAvailableSpeakers")]
    //         public Speaker Speaker;

    //         [ValueDropdown("GetAvailableSpeeds")]
    //         public float Speed = 1f;

    //         [TextArea] public string Text;

    // #if UNITY_EDITOR

    //         private List<Speaker> GetAvailableSpeakers()
    //         {
    //             if (Application.isPlaying)
    //             {
    //                 return DialogueManager.Instance?.Characters?.availableSpeakers ?? new List<Speaker>();
    //             }
    //             else
    //             {
    //                 // В режиме редактирования ищем настройки в сцене или ресурсах
    //                 var settings = FindObjectOfType<DialogueManager>()?.Characters;
    //                 if (settings == null)
    //                 {
    //                     settings = Resources.Load<DialogueSpeakers>("Characters");
    //                 }
    //                 return settings?.availableSpeakers ?? new List<Speaker>();
    //             }
    //         }

    //         private List<float> GetAvailableSpeeds()
    //         {
    //             if (Application.isPlaying)
    //             {
    //                 return DialogueManager.Instance?.TypingSpeeds?.availableSpeeds ?? new List<float> { 1f };
    //             }
    //             else
    //             {
    //                 var settings = FindObjectOfType<DialogueManager>()?.TypingSpeeds;
    //                 if (settings == null)
    //                 {
    //                     settings = Resources.Load<TypingSpeeds>("TypingSpeeds");
    //                 }
    //                 return settings?.availableSpeeds ?? new List<float> { 1f };
    //             }
    //         }
    // #endif
    //     }

    // [System.Serializable]
    // public class Dialogue
    // {
    //     public int ID;
    //     public bool IsRead;
    //     public List<Sentence> Sentences;
    // }

    // [Header("Dialogues")]
    // [SerializeField] private List<Dialogue> dialogues;
    [SerializeField] private DialogueData dialogueData;
    private List<Dialogue> dialogues;

    public int DialoguesCount => dialogues.Count;
    private List<Sentence> currentSentences;
    private int currentIndex;
    private Coroutine typingCoroutine;

    [Header("Events")]
    public UnityEvent OnDialogueStarted;
    public UnityEvent OnDialogueEnded;
    public UnityEvent<Sentence> OnSentenceChanged;

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

        dialogues = dialogueData.dialogues;

        ValidateDialogues();
    }

    private void ValidateDialogues()
    {
        foreach (var dialogue in dialogues)
        {
            if (dialogue.Sentences == null || dialogue.Sentences.Count == 0)
            {
                Debug.LogWarning($"Dialogue {dialogue.ID} has no sentences!");
            }
        }
    }

    public void StartDialogue(int dialogueID)
    {
        Dialogue dialogue = GetDialogue(dialogueID);
        if (!ValidateDialogue(dialogue)) return;

        PrepareDialogue(dialogue);
        view.ToggleDialogueBox(true);
        OnDialogueStarted?.Invoke();
        StartTyping(currentSentences[0]);
    }

    private Dialogue GetDialogue(int id) => dialogues.Find(d => d.ID == id);

    private bool ValidateDialogue(Dialogue dialogue)
    {
        if (dialogue == null)
        {
            Debug.LogWarning($"Dialogue not found");
            return false;
        }

        if (dialogue.Sentences == null || !dialogue.Sentences.Any())
        {
            Debug.LogWarning($"Dialogue {dialogue.ID} has no sentences");
            return false;
        }

        return true;
    }

    private void PrepareDialogue(Dialogue dialogue)
    {
        currentSentences = dialogue.Sentences;
        currentIndex = 0;
        dialogue.IsRead = true;
    }

    private void StartTyping(Sentence sentence)
    {
        if (sentence == null) return;

        StopTyping();
        SetupDialogueBox(sentence.Speaker);
        typingCoroutine = StartCoroutine(TypeSentence(sentence));
    }

    private void StopTyping()
    {
        if (typingCoroutine == null) return;

        StopCoroutine(typingCoroutine);
        typingCoroutine = null;
    }

    private IEnumerator TypeSentence(Sentence sentence)
    {
        var fullText = sentence.Text;

        for (int i = 0; i < fullText.Length; i++)
        {
            view.SetDialogueText(fullText.Substring(0, i + 1));
            yield return new WaitForSeconds(Time.deltaTime * sentence.Speed);
        }

        typingCoroutine = null;
    }

    private void SetupDialogueBox(Speaker speaker)
    {
        if (speaker == null) return;

        view.UpdateView(speaker.Name);
        view.UpdateDialoguePosition(speaker.Transform);
        OnSentenceChanged?.Invoke(currentSentences[currentIndex]);
    }

    public void NextSentence()
    {
        if (++currentIndex < currentSentences.Count)
            StartTyping(currentSentences[currentIndex]);
        else
            EndDialogue();
    }

    public void EndDialogue()
    {
        StopTyping();
        view.CloseDialogueBox();
        OnDialogueEnded?.Invoke();
    }

    public void FastForward()
    {
        if (typingCoroutine != null)
        {
            StopTyping();
            view.SetDialogueText(currentSentences[currentIndex].Text);
        }
    }
}