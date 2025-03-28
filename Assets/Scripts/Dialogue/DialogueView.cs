using System.Text;
using TMPro;
using UnityEngine;

public class DialogueView : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] private AnimatorToggler buttonToggler;
    private AnimatorToggler boxToggler;
    [SerializeField] private RectTransform dialogueBox;
    [SerializeField] private TextMeshProUGUI nameText;
    private const string MISSING_NAME = "???";
    [SerializeField] private TextMeshProUGUI dialogueText;
    private readonly StringBuilder textBuilder = new();
    [SerializeField] private Vector2 defaultPosition = new Vector2(0, 1.5f);
    [SerializeField] private Vector2 dialogueOffset = new Vector2(0, 115f);
    [SerializeField] private float screenMargin = 0.22f;
    private Vector2 screenSize;

    private void Awake()
    {
        boxToggler = dialogueBox.gameObject.GetComponent<AnimatorToggler>();
        mainCamera = Camera.main;
    }

    private void Start() => screenSize = new Vector2(Screen.width, Screen.height);

    public void ToggleDialogueBox(bool isVisible)
    {
        buttonToggler?.SetActive(!isVisible);
        boxToggler?.SetActive(isVisible);
    }

    public void CloseDialogueBox() => boxToggler?.SetActive(false);

    public void UpdateView(string name)
    {
        nameText.text = string.IsNullOrEmpty(name) ? MISSING_NAME : name;
        SetDialogueText(string.Empty);
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
}