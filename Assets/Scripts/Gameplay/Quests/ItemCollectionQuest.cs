using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemCollectionQuest : Quest
{
    [Header("Item Collection Settings")]
    [SerializeField] private int requiredItemsCount = 4;
    [SerializeField] private List<string> validItemIDs = new();

    private int _itemsCollected;
    [SerializeField] public UnityEvent<string> onItemCollected;

    protected override void Awake()
    {
        base.Awake();
        onItemCollected.AddListener(DialogueManager.Instance.StartDialogueBranch);
    }

    protected override void InitializeQuest()
    {
        _itemsCollected = 0;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsActive || other.CompareTag("Player") || !other.TryGetComponent(out SceneItem item)) return;

        if (IsCompleted)
        {
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
            CompleteQuest();
        }
        else
        {
            UpdateProgress();
            UpdateProgress(item.ID);
        }
    }

    private void UpdateProgress(string itemID)
    {
        onItemCollected?.Invoke(itemID);
    }
}