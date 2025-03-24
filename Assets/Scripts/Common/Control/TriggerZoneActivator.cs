using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class TriggerZoneActivator : MonoBehaviour
{
    [Header("Trigger Events")]
    [SerializeField] private UnityEvent onTriggerEnterEvent;
    [SerializeField] private UnityEvent onTriggerExitEvent;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            onTriggerEnterEvent?.Invoke();
    }
        // DialogueManager.Instance.SetStartDialogueButtonAnimation(true);

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            onTriggerExitEvent?.Invoke();
    }
    // DialogueManager.Instance.SetStartDialogueButtonAnimation(false);
    // DialogueManager.EndDialogue();
}