using System.Collections.Generic;
using UnityEngine;

public class Quests : MonoBehaviour
{
    [HideInInspector] public int idPickedUp;
    public Animator startDialogueButtonAnim;
    [HideInInspector] public int questNumber;
    [SerializeField] private List<int> dialogueIds = new List<int>{ 1, 2, 3, 4, 6 };

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) return; // Если это игрок, выходим из метода

        ItemOnScene pickup = other.GetComponent<ItemOnScene>();
        if (pickup != null)
        {
            idPickedUp = pickup.id;
            questNumber++;
            Destroy(other.gameObject);

            // Проверяем, есть ли соответствующий диалог для текущего idPickedUp
            if (idPickedUp >= 0 && idPickedUp < dialogueIds.Count)
            {
                StartQuestDialogue(dialogueIds[idPickedUp]);
            }

            if (questNumber == 4)
            {
                DialogManager.Instance.StartDialogue(5);
            }
        }
    }

    private void StartQuestDialogue(int dialogueId)
    {
        DialogManager.Instance.SetStartDialogueButtonAnimation(true);
        DialogManager.Instance.StartDialogue(dialogueId);
    }
}
