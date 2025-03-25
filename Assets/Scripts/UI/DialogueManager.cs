using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [System.Serializable]
    public class Speaker
    {
        public string name;
        public Transform transform;
        public Vector2 dialogueOffset = new Vector2(0, 115f);
    }

    [System.Serializable]
    public class TypingSpeedPreset
    {
        public string name;
        public float speedMultiplier = 1f;
    }

    [System.Serializable]
    public class Sentence
    {
        public Speaker speaker;
        public string text;
        public TypingSpeedPreset speedPreset;
    }

    [System.Serializable]
    public class Dialog
    {
        public int id;
        public bool isRead;
        public List<Sentence> sentences;
    }

    [Header("References")]
    [SerializeField] private AnimatorToggler buttonToggler;
    [SerializeField] private AnimatorToggler boxToggler;
    [SerializeField] private RectTransform dialogueBox;
    [SerializeField] private Text nameText;
    [SerializeField] private Text dialogueText;

    [Header("Settings")]
    [SerializeField] private Vector2 defaultPosition = new Vector2(0, 1.5f);
    [SerializeField] private float screenMargin = 0.35f;
    [SerializeField] private float baseTypingSpeed = 0.05f;
    [SerializeField] private List<TypingSpeedPreset> speedPresets;
    [SerializeField] private List<Speaker> speakers;
    [SerializeField] private List<Dialog> dialogues;

    public int DialoguesCount => dialogues.Count;
    private List<Sentence> currentSentences;
    private int currentIndex;
    private Camera mainCamera;
    private Coroutine typingCoroutine;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        mainCamera = Camera.main;
    }

    public void StartDialogue(int dialogId)
    {
        var dialog = GetDialog(dialogId);
        if (dialog == null)
        {
            Debug.LogWarning($"Dialog {dialogId} not found");
            return;
        }

        PrepareDialogue(dialog);
    }

    private Dialog GetDialog(int id) => dialogues.Find(d => d.id == id);

    private void PrepareDialogue(Dialog dialog)
    {
        buttonToggler?.SetActive(false);
        boxToggler?.SetActive(true);

        currentSentences = dialog.sentences;
        currentIndex = 0;

        StartTyping(currentSentences[0]);
    }

    private void StartTyping(Sentence sentence)
    {
        if (sentence == null) return;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeSentence(sentence));
    }

    private IEnumerator TypeSentence(Sentence sentence)
    {
        nameText.text = sentence.speaker.name;
        dialogueText.text = string.Empty;
        UpdateDialoguePosition(sentence.speaker);

        float speed = baseTypingSpeed * (sentence.speedPreset?.speedMultiplier ?? 1f);
        var text = sentence.text;

        for (int i = 0; i < text.Length; i++)
        {
            dialogueText.text = text[..(i + 1)];
            yield return new WaitForSeconds(speed);
        }
    }

    private void UpdateDialoguePosition(Speaker speaker)
    {
        if (dialogueBox == null || mainCamera == null) return;

        if (speaker?.transform != null)
        {
            var screenPos = mainCamera.WorldToScreenPoint(speaker.transform.position) + (Vector3)speaker.dialogueOffset;
            dialogueBox.position = AdjustPosition(screenPos);
        }
        else
        {
            dialogueBox.anchoredPosition = defaultPosition;
        }
    }

    private Vector2 AdjustPosition(Vector2 position)
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
        buttonToggler?.SetActive(false);
        boxToggler?.SetActive(false);
    }
}