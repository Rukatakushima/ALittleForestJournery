using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static DialogueSpeakers;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Configuration")]
    public DialogueSpeakers Characters;
    public TypingSpeeds TypingSpeeds;

    [System.Serializable]
    public class Sentence
    {
        [ValueDropdown("GetAvailableSpeakers")]
        public Speaker Speaker;

        [ValueDropdown("GetAvailableSpeeds")]
        public float Speed = 1f;

        [TextArea] public string Text;

#if UNITY_EDITOR

        private List<Speaker> GetAvailableSpeakers()
        {
            if (Application.isPlaying)
            {
                return DialogueManager.Instance?.Characters?.availableSpeakers ?? new List<Speaker>();
            }
            else
            {
                // В режиме редактирования ищем настройки в сцене или ресурсах
                var settings = FindObjectOfType<DialogueManager>()?.Characters;
                if (settings == null)
                {
                    settings = Resources.Load<DialogueSpeakers>("Characters");
                }
                return settings?.availableSpeakers ?? new List<Speaker>();
            }
        }

        private List<float> GetAvailableSpeeds()
        {
            if (Application.isPlaying)
            {
                return DialogueManager.Instance?.TypingSpeeds?.availableSpeeds ?? new List<float> { 1f };
            }
            else
            {
                var settings = FindObjectOfType<DialogueManager>()?.TypingSpeeds;
                if (settings == null)
                {
                    settings = Resources.Load<TypingSpeeds>("TypingSpeeds");
                }
                return settings?.availableSpeeds ?? new List<float> { 1f };
            }
        }
#endif
    }

    [System.Serializable]
    public class Dialogue
    {
        public int ID;
        public bool IsRead;
        public List<Sentence> Sentences;
    }

    [Header("References")]
    [SerializeField] private AnimatorToggler buttonToggler;
    [SerializeField] private AnimatorToggler boxToggler;
    [SerializeField] private RectTransform dialogueBox;
    [SerializeField] private Text nameText;
    [SerializeField] private Text dialogueText;

    [Header("Settings")]
    [SerializeField] private Vector2 defaultPosition = new Vector2(0, 1.5f);
    [SerializeField] private Vector2 dialogueOffset = new Vector2(0, 115f);
    [SerializeField] private float screenMargin = 0.22f;
    [SerializeField] private List<Dialogue> dialogues;

    public UnityEvent OnDialogueStarted;
    public UnityEvent OnDialogueEnded;
    public UnityEvent<Sentence> OnSentenceChanged;

    public int DialoguesCount => dialogues.Count;
    private List<Sentence> currentSentences;
    private int currentIndex;
    private Camera mainCamera;
    private Coroutine typingCoroutine;
    private const string MISSING_NAME = "???";

    public bool IsDialogueActive => boxToggler != null && boxToggler.IsActive;
    public Sentence CurrentSentence => currentIndex >= 0 && currentIndex < currentSentences?.Count ? currentSentences[currentIndex] : null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Initialize();
    }

    private void Initialize()
    {
        mainCamera = Camera.main;
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
        ToggleDialogueBox(true);
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

    private void ToggleDialogueBox(bool isVisible)
    {
        buttonToggler?.SetActive(!isVisible);
        boxToggler?.SetActive(isVisible);
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
            dialogueText.text = fullText.Substring(0, i + 1);
            yield return new WaitForSeconds(Time.deltaTime * sentence.Speed);
        }

        typingCoroutine = null;
    }

    private void SetupDialogueBox(Speaker speaker)
    {
        nameText.text = string.IsNullOrEmpty(speaker?.Name) ? MISSING_NAME : speaker.Name;
        dialogueText.text = string.Empty;
        UpdateDialoguePosition(speaker);
        OnSentenceChanged?.Invoke(currentSentences[currentIndex]);
    }

    private void UpdateDialoguePosition(Speaker speaker)
    {
        if (dialogueBox == null || mainCamera == null) return;

        if (speaker?.Transform != null)
        {
            Vector2 screenPos = (Vector2)mainCamera.WorldToScreenPoint(speaker.Transform.position) + dialogueOffset;
            dialogueBox.position = ClampPositionToScreen(screenPos);
        }
        else
            dialogueBox.anchoredPosition = defaultPosition;
    }

    private Vector2 ClampPositionToScreen(Vector2 position)
    {
        if (dialogueBox == null) return position;

        Vector2 min = dialogueBox.sizeDelta * screenMargin;
        Vector2 max = new Vector2(Screen.width, Screen.height) - min;

        return new Vector2(Mathf.Clamp(position.x, min.x, max.x), Mathf.Clamp(position.y, min.y, max.y));
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
        boxToggler?.SetActive(false);
        OnDialogueEnded?.Invoke();
    }

    public void FastForward()
    {
        if (typingCoroutine != null)
        {
            StopTyping();
            dialogueText.text = currentSentences[currentIndex].Text;
        }
    }
}