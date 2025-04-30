using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class Quest : MonoBehaviour
{
    [Header("Base Quest Settings")]
    [SerializeField] public UnityEvent onInteract;
    [SerializeField] public UnityEvent onQuestStarted;
    [SerializeField] public UnityEvent onQuestProgress;
    [SerializeField] public UnityEvent onQuestInProgress;
    [SerializeField] public UnityEvent onQuestCompleted;
    [SerializeField] public UnityEvent onQuestAfterCompleted;

    protected bool IsActive, IsCompleted;
    
    [Header("Start Button")]
    [SerializeField] private Button startDialogueButton;
    private TriggerZoneActivator _triggerZoneActivator;
    // [SerializeField] private Quest quest;

    #region TEST_MARKS
    protected virtual void Awake()
    {
        onInteract.AddListener(() => WriteLog("interacted"));
        onQuestStarted.AddListener(() => WriteLog("started"));
        onQuestProgress.AddListener(() => WriteLog("progressing"));
        onQuestInProgress.AddListener(() => WriteLog("in progress"));
        onQuestCompleted.AddListener(() => WriteLog("completed"));
        onQuestAfterCompleted.AddListener(() => WriteLog("after wording"));
        
        startDialogueButton.onClick.AddListener(OnStartDialogButtonPressed);
        _triggerZoneActivator =  GetComponent<TriggerZoneActivator>();
    }

    private void WriteLog(string words)
    {
        Debug.Log("Quest is " + words);
    }
    #endregion

    public void Interact()
    {
        onInteract?.Invoke();

        if (IsCompleted)
            AfterFinishQuest();
        else if (!IsCompleted && IsActive)
            InProgressQuest();
        else if (!IsActive)
            StartQuest();
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

    protected abstract void InitializeQuest();

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

    private void OnStartDialogButtonPressed()
    {
        if (_triggerZoneActivator.IsPlayerInZone && this != null)
        {
            Interact();
        }
    }
}