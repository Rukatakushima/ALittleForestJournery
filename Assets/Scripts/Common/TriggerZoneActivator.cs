using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class TriggerZoneActivator : MonoBehaviour
{
    [Header("Trigger Events")]
    [SerializeField] private UnityEvent onTriggerEnterEvent;
    [SerializeField] private UnityEvent onTriggerExitEvent;
    
    public bool IsPlayerInZone { get; private set; }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        IsPlayerInZone = true;
        onTriggerEnterEvent?.Invoke();
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        IsPlayerInZone = false;
        onTriggerExitEvent?.Invoke();
    }
}