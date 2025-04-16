using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using static DialogueData;

public class DialogueView : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private CharacterRegistry characterRegistry;
    private Camera mainCamera;
    // [SerializeField] private AnimatorToggler buttonToggler;
    // private AnimatorToggler boxToggler;
    [SerializeField] private RectTransform dialogueBox;
    [SerializeField] private TextMeshProUGUI nameText;
    private const string MISSING_NAME = "???";
    [SerializeField] private TextMeshProUGUI dialogueText;
    private readonly StringBuilder textBuilder = new();
    [SerializeField] private Vector2 defaultPosition = new Vector2(0, 1.5f);
    [SerializeField] private Vector2 dialogueOffset = new Vector2(0, 115f);
    [SerializeField] private float screenMargin = 0.22f;
    private Vector2 screenSize;
    private Coroutine typingCoroutine;

    private void Awake()
    {
        mainCamera = Camera.main;
        // boxToggler = dialogueBox.gameObject.GetComponent<AnimatorToggler>();

        dialogueManager ??= DialogueManager.Instance;
        if (dialogueManager != null)
            dialogueManager.OnDialogueLineActive.AddListener(UpdateDialogueBox);
        else
            Debug.LogWarning("DialogueManager is not found");

        characterRegistry ??= FindObjectOfType<CharacterRegistry>();
        if (characterRegistry == null)
            Debug.LogError("CharacterRegistry not found!");
    }

    private void Start() => screenSize = new Vector2(Screen.width, Screen.height);

    // public void ToggleDialogueBox(bool isVisible)
    // {
    //     buttonToggler?.SetActive(!isVisible);
    //     boxToggler?.SetActive(isVisible);
    // }

    public void UpdateDialogueBox(DialogueLine dialogueLine, int sentenceIndex)
    {
        StopTyping();

        nameText.text = string.IsNullOrEmpty(dialogueLine.speaker.Name) ?
            MISSING_NAME : dialogueLine.speaker.Name;

        var speakerTransform = characterRegistry?.GetCharacterTransform(dialogueLine.speaker.SpeakerID);
        UpdateDialoguePosition(speakerTransform);

        typingCoroutine = StartCoroutine(TypeSentence(dialogueLine.Sentences[sentenceIndex], dialogueLine.Speed));
    }

    public void SetDialogueText(string text)
    {
        textBuilder.Clear();
        textBuilder.Append(text);
        dialogueText.text = textBuilder.ToString();
    }

    public void UpdateDialoguePosition(Transform speakerTransform)
    {
        if (dialogueBox == null || mainCamera == null) return;

        if (speakerTransform != null)
        {
            Vector2 screenPos = (Vector2)mainCamera.WorldToScreenPoint(speakerTransform.position) + dialogueOffset;
            dialogueBox.position = ClampPositionToScreen(screenPos);
        }
        else
            dialogueBox.anchoredPosition = defaultPosition;
    }

    private Vector2 ClampPositionToScreen(Vector2 position)
    {
        if (dialogueBox == null) return position;

        Vector2 min = dialogueBox.sizeDelta * screenMargin;
        Vector2 max = screenSize - min;

        return new Vector2(Mathf.Clamp(position.x, min.x, max.x), Mathf.Clamp(position.y, min.y, max.y));
    }

    private IEnumerator TypeSentence(string text, float speed)
    {
        var waitTime = new WaitForSecondsRealtime(Time.deltaTime * speed);

        for (int i = 0; i < text.Length; i++)
        {
            SetDialogueText(text.Substring(0, i + 1));
            yield return waitTime;
        }

        typingCoroutine = null;
    }

    private void StopTyping()
    {
        if (typingCoroutine == null) return;

        StopCoroutine(typingCoroutine);
        typingCoroutine = null;
        SetDialogueText(string.Empty);
    }
}