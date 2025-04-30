using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemCollectionQuest : Quest
{
    [Header("Item Collection Settings")]
    [SerializeField] private int requiredItemsCount = 4;
    [SerializeField] private List<int> validItemIDs = new();
    // [SerializeField] private string questDialogueName;
    // [SerializeField] private int startDialogueID = 0;
    // [SerializeField] private int completionDialogueID = 5;
    // [SerializeField] private int middleProgressedDialogueID = 6;
    // [SerializeField] private int afterwordDialogueID = 7;

    private int _itemsCollected;
    [SerializeField] public UnityEvent<int> onItemCollected;

    protected override void Awake()
    {
        base.Awake();
        onItemCollected.AddListener(DialogueManager.Instance.StartDialogue);
    }

    protected override void InitializeQuest()
    {
        _itemsCollected = 0;
        // DialogueManager.Instance.SetCurrentDialogueData(questDialogueName);
        // OpenQuestDialogue(startDialogueID);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsActive || other.CompareTag("Player") || !other.TryGetComponent(out SceneItem item)) return;

        if (IsCompleted)
        {
            // OpenQuestDialogue(afterwordDialogueID);
            AfterFinishQuest();
            return;
        }

        if (validItemIDs.Contains(item.ID))
        {
            ItemPickup(item);
        }
        else
            Debug.LogWarning($"Item ID {item.ID} is not part of this quest!");
    }

    private void ItemPickup(SceneItem item)
    {
        _itemsCollected++;
        item.Hide();

        if (_itemsCollected == requiredItemsCount)
        {
            // OpenQuestDialogue(completionDialogueID);
            CompleteQuest();
        }
        else
        {
            // OpenQuestDialogue(item.ID);
            UpdateProgress();
            UpdateProgress(item.ID);
        }
    }

    private void UpdateProgress(int itemID)
    {
        onItemCollected?.Invoke(itemID);
    }

    // public void OpenQuestDialogue(int dialogueID)
    // {
    //     DialogueManager.Instance.StartDialogue(/*questDialogueName,*/ dialogueID);
    // }

    // protected override void InProgressQuest()
    // {
    //     base.InProgressQuest();
    //     OpenQuestDialogue(middleProgressedDialogueID);
    // }

    // protected override void AfterFinishQuest() { base.AfterFinishQuest(); OpenQuestDialogue(afterwordDialogueID); }
}