using UnityEngine;
using UnityEngine.Events;

public class Quests : MonoBehaviour
{
    [SerializeField] private int requiredItemsCount = 4;
    [SerializeField] private int specialDialogueID = 5;
    private int itemsPickedUp;

    [SerializeField] private UnityEvent startQuestDialogueEvent;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) return;

        SceneItem item = other.GetComponent<SceneItem>();
        if (item != null) ItemPickup(item);
    }

    private void ItemPickup(SceneItem item)
    {
        item.Hide();
        itemsPickedUp++;

        if (itemsPickedUp == requiredItemsCount && IsValidDialogueID(specialDialogueID))
        {
            StartQuestDialogue(specialDialogueID);
            return;
        }

        StartQuestDialogue(item.ID);
    }

    private bool IsValidDialogueID(int id) => id >= 0 && id < DialogueManager.Instance.DialogsQueue.Count;

    public void StartQuestDialogue(int dialogueID)
    {
        if (IsValidDialogueID(dialogueID))
        {
            startQuestDialogueEvent?.Invoke();
            DialogueManager.Instance.StartDialogue(dialogueID);
        }
    }
}