using UnityEngine;

public class Quests : MonoBehaviour
{
    [HideInInspector] public int idPickedUp;
    public Animator startDialogueButtonAnim;
    [HideInInspector] public int questNumber;
    private int[] dialogueIds = { 1, 2, 3, 4 };

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) return; // Если это игрок, выходим из метода

        Pickup pickup = other.GetComponent<Pickup>();
        if (pickup != null)
        {
            idPickedUp = pickup.id;
            questNumber++;
            Destroy(other.gameObject);

            // Проверяем, есть ли соответствующий диалог для текущего idPickedUp
            if (idPickedUp >= 0 && idPickedUp < dialogueIds.Length)
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
