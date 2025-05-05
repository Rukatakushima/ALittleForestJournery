using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class Quest : MonoBehaviour
{
    [Header("Start Button")]
    [SerializeField] private Button startDialogueButton;
    [SerializeField] private DialogueData dialogueData;
    private TriggerZoneActivator _triggerZoneActivator;

    [Header("Base Quest Settings")]
    [SerializeField] public UnityEvent onInteract;
    [SerializeField] public UnityEvent onQuestStarted;
    [SerializeField] public UnityEvent onQuestProgress;
    [SerializeField] public UnityEvent onQuestInProgress;
    [SerializeField] public UnityEvent onQuestCompleted;
    [SerializeField] public UnityEvent onQuestAfterCompleted;

    protected bool IsActive, IsCompleted;

    #region TEST_MARKS
    protected virtual void Awake()
    {
        // onInteract.AddListener(SetDialogueData);
        onInteract.AddListener(() => WriteLog("interacted"));
        onQuestStarted.AddListener(() => WriteLog("started"));
        onQuestProgress.AddListener(() => WriteLog("progressing"));
        onQuestInProgress.AddListener(() => WriteLog("in progress"));
        onQuestCompleted.AddListener(() => WriteLog("completed"));
        onQuestAfterCompleted.AddListener(() => WriteLog("after wording"));
        
        startDialogueButton.onClick.AddListener(Interact);
        _triggerZoneActivator =  GetComponent<TriggerZoneActivator>();
    }

    private void WriteLog(string words)
    {
        Debug.Log("Quest is " + words);
    }
    #endregion

    public void Interact()
    {
        if (!_triggerZoneActivator.IsPlayerInZone || this == null) return;

        onInteract?.Invoke();

        switch (IsCompleted)
        {
            case true:
                AfterFinishQuest();
                break;
            case false when IsActive:
                InProgressQuest();
                break;
            default:
            {
                if (!IsActive)
                    StartQuest();
                break;
            }
        }
    }

    private void SetDialogueData()
    {
        DialogueManager.Instance.SetDialogue(dialogueData);
    }

    public void StartQuest()
    {
        if (IsCompleted)
        {
            AfterFinishQuest();
        }
        else if (IsActive && IsCompleted)
        {
            InProgressQuest();
        }
        else
        {
            IsActive = true;
            InitializeQuest();
            onQuestStarted?.Invoke();
        }
    }

    protected virtual void InitializeQuest()
    {
        SetDialogueData();
    }

    protected virtual void UpdateProgress()
    {
        onQuestProgress?.Invoke();
    }

    public void CompleteQuest()
    {
        if (!IsActive || IsCompleted) return;

        IsCompleted = true;
        IsActive = false;
        onQuestCompleted?.Invoke();
        CleanUpQuest();
    }

    protected virtual void CleanUpQuest() { }

    protected virtual void InProgressQuest() { onQuestInProgress?.Invoke(); }

    protected virtual void AfterFinishQuest() { onQuestAfterCompleted?.Invoke(); }
}