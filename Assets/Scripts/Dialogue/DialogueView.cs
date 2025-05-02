using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using static DialogueData;

public class DialogueView : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private CharacterRegistry characterRegistry;
    private Camera _mainCamera;
    [SerializeField] private RectTransform dialogueBox;
    [SerializeField] private TextMeshProUGUI nameText;
    private const string MISSING_NAME = "???";
    [SerializeField] private TextMeshProUGUI dialogueText;
    private readonly StringBuilder _textBuilder = new();
    [SerializeField] private Vector2 defaultPosition = new Vector2(0, 1.5f);
    [SerializeField] private Vector2 dialogueOffset = new Vector2(0, 115f);
    [SerializeField] private float screenMargin = 0.22f;
    private Vector2 _screenSize;
    private Coroutine _typingCoroutine;

    private void Awake()
    {
        _mainCamera = Camera.main;

        dialogueManager ??= DialogueManager.Instance;
        if (dialogueManager != null)
        {
            dialogueManager.onChoiceNodeActive.AddListener(UpdateDialogueBox);
            dialogueManager.onDialogueNodeActive.AddListener(UpdateDialogueBox);
        }
        else
            Debug.LogWarning("DialogueManager is not found");

        characterRegistry ??= FindObjectOfType<CharacterRegistry>();
        if (characterRegistry == null)
            Debug.LogError("CharacterRegistry not found!");
    }

    private void Start() => _screenSize = new Vector2(Screen.width, Screen.height);

    private void UpdateDialogueBox(DialogueNode dialogueNode, int sentenceIndex)
    {
        StopTyping();

        SetDialogueName(dialogueNode.speaker.speakerName);

        var speakerTransform = !string.IsNullOrEmpty(dialogueNode.speaker.speakerID) ?
            (characterRegistry?.GetCharacterTransform(dialogueNode.speaker.speakerID)) : null;
        UpdateDialoguePosition(speakerTransform);

        _typingCoroutine = StartCoroutine(TypeSentence(dialogueNode.sentences[sentenceIndex], dialogueNode.speed));
    }

    private void SetDialogueName(string dialogueName) => nameText.text = string.IsNullOrEmpty(dialogueName) ? MISSING_NAME : dialogueName;

    private void SetDialogueText(string text)
    {
        _textBuilder.Clear();
        _textBuilder.Append(text);
        dialogueText.text = _textBuilder.ToString();
    }

    private void UpdateDialoguePosition(Transform characterTransform)
    {
        if (dialogueBox == null || _mainCamera == null) return;

        if (characterTransform != null)
        {
            Vector2 screenPos = (Vector2)_mainCamera.WorldToScreenPoint(characterTransform.position) + dialogueOffset;
            dialogueBox.position = ClampPositionToScreen(screenPos);
        }
        else
            dialogueBox.anchoredPosition = defaultPosition;
    }

    private Vector2 ClampPositionToScreen(Vector2 position)
    {
        if (dialogueBox == null) return position;

        Vector2 min = dialogueBox.sizeDelta * screenMargin;
        Vector2 max = _screenSize - min;

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

        _typingCoroutine = null;
    }

    private void StopTyping()
    {
        if (_typingCoroutine == null) return;

        StopCoroutine(_typingCoroutine);
        _typingCoroutine = null;
        SetDialogueText(string.Empty);
    }
}