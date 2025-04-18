using UnityEngine;
using UnityEngine.Events;

public abstract class Quest : MonoBehaviour
{
    [Header("Base Quest Settings")]
    [SerializeField] public UnityEvent onInteract;
    [SerializeField] public UnityEvent onQuestStarted;
    [SerializeField] public UnityEvent onQuestProgress;
    [SerializeField] public UnityEvent onQuestInProgress;
    [SerializeField] public UnityEvent onQuestCompleted;
    [SerializeField] public UnityEvent onQuestAfterCompleted;

    protected bool isActive, isCompleted = false;

    #region TEST_MARKS
    protected virtual void Awake()
    {
        onInteract.AddListener(() => WriteLog("interacted"));
        onQuestStarted.AddListener(() => WriteLog("started"));
        onQuestProgress.AddListener(() => WriteLog("progressing"));
        onQuestInProgress.AddListener(() => WriteLog("in progress"));
        onQuestCompleted.AddListener(() => WriteLog("completed"));
        onQuestAfterCompleted.AddListener(() => WriteLog("after wording"));
    }

    private void WriteLog(string words)
    {
        Debug.Log("Quest is " + words);
    }
    #endregion

    public void Interact()
    {
        onInteract?.Invoke();

        if (isCompleted)
            AfterFinishQuest();
        else if (!isCompleted && isActive)
            InProgressQuest();
        else if (!isActive)
            StartQuest();
    }

    public void StartQuest()
    {
        if (isCompleted)
        {
            AfterFinishQuest();
        }
        else if (isActive && isCompleted)
        {
            InProgressQuest();
        }
        else
        {
            isActive = true;
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
        if (!isActive || isCompleted) return;

        isCompleted = true;
        isActive = false;
        onQuestCompleted?.Invoke();
        CleanUpQuest();
    }

    protected virtual void CleanUpQuest() { }

    protected virtual void InProgressQuest() { onQuestInProgress?.Invoke(); }

    protected virtual void AfterFinishQuest() { onQuestAfterCompleted?.Invoke(); }
}