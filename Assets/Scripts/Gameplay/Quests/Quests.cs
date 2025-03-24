using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Quests : MonoBehaviour
{
    [SerializeField] private List<int> dialogueIDs = new List<int> { 1, 2, 3, 4, 6 };
    [SerializeField] private int requiredItemsCount = 4;
    [SerializeField] private int specialDialogueID = 5;
    [HideInInspector] public int itemsPickedUp { get; private set; }

    [SerializeField] private UnityEvent startQuestDialogueEvent;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) return;

        SceneItem pickup = other.GetComponent<SceneItem>();
        if (pickup == null) return;

        other.gameObject.SetActive(false);

        int itemID = pickup.id;
        itemsPickedUp++;

        if (itemID >= 0 && itemID < dialogueIDs.Count)
        {
            startQuestDialogueEvent?.Invoke();
            DialogueManager.Instance.StartDialogue(dialogueIDs[itemID]);
        }

        if (itemsPickedUp == requiredItemsCount)
            DialogueManager.Instance.StartDialogue(specialDialogueID);
    }

    // private void StartQuestDialogue(int dialogueId)
    // {
    //     startQuestDialogueEvent?.Invoke();
    // DialogueManager.Instance.SetStartDialogueButtonAnimation(true);
    // DialogueManager.Instance.StartDialogue(dialogueId);
    // }
}