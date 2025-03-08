using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    [SerializeField] private Animator startDialogueButtonAnim;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            DialogManager.Instance.SetStartDialogueButtonAnimation(true);
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        DialogManager.Instance.SetStartDialogueButtonAnimation(false);
        DialogManager.EndDialogue();
    }
}