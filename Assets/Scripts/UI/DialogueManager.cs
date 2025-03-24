using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Serializable]
    public class Dialogue
    {
        public int id;
        public bool isRead;
        public List<Sentence> SentencesList;
    }

    [Serializable]
    public class Sentence
    {
        public string characterName;
        public string text;
        public float typingSpeed;
    }

    [Header("References")]
    [SerializeField] private AnimatorToggler buttonToggler;
    [SerializeField] private AnimatorToggler boxToggler;
    [SerializeField] private RectTransform dialogueBox;
    [SerializeField] private Text speakerName;
    [SerializeField] private Text dialogueText;
    [SerializeField] private Transform speakerTransform;

    [Header("Settings")]
    [SerializeField] private Vector2 defaultDialoguePosition = new Vector2(0, 1.5f);
    [SerializeField] private Vector2 characterDialogueOffset = new Vector2(0, 115f);
    [SerializeField] private float dialogueSizeMargin = 0.35f;
    [SerializeField] private List<Dialogue> Dialogues;

    public int DialoguesCount => Dialogues.Count;
    private int currentSentenceIndex;
    private List<Sentence> currentSentences;
    private Camera mainCamera;
    private Coroutine typingCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
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

        SetupDialogue(dialog);
    }

    private Dialogue GetDialog(int id) => Dialogues.Find(d => d.id == id);

    private void SetupDialogue(Dialogue dialog)
    {
        buttonToggler?.SetActive(false);
        boxToggler?.SetActive(true);

        currentSentences = dialog.SentencesList;
        currentSentenceIndex = 0;

        StartTyping(currentSentences[0]);
    }

    private void StartTyping(Sentence sentence)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeSentence(sentence));
    }

    private IEnumerator TypeSentence(Sentence sentence)
    {
        speakerName.text = sentence.characterName;
        dialogueText.text = string.Empty;

        UpdateDialogueBoxPosition(sentence.characterName);

        var text = sentence.text;
        for (int i = 0; i < text.Length; i++)
        {
            dialogueText.text = text.Substring(0, i + 1);
            yield return new WaitForSeconds(sentence.typingSpeed * Time.deltaTime);
        }
    }

    private void UpdateDialogueBoxPosition(string speakerName)
    {
        if (dialogueBox == null || mainCamera == null) return;

        if (speakerName == speakerTransform?.name)
        {
            Vector2 screenPosition = (Vector2)mainCamera.WorldToScreenPoint(speakerTransform.position) + characterDialogueOffset;
            dialogueBox.position = AdjustPosition(screenPosition);
        }
        else
            dialogueBox.anchoredPosition = defaultDialoguePosition;
    }

    private Vector2 AdjustPosition(Vector2 position)
    {
        if (dialogueBox == null) return position;

        Vector2 min = dialogueBox.sizeDelta * dialogueSizeMargin;
        Vector2 max = new Vector2(Screen.width, Screen.height) - min;

        return new Vector2(Mathf.Clamp(position.x, min.x, max.x), Mathf.Clamp(position.y, min.y, max.y));
    }

    public void DisplayNextSentence()
    {
        if (++currentSentenceIndex < currentSentences.Count)
            StartTyping(currentSentences[currentSentenceIndex]);
        else
            EndDialogue();
    }

    public void EndDialogue()
    {
        buttonToggler?.SetActive(false);
        boxToggler?.SetActive(false);
    }
}
