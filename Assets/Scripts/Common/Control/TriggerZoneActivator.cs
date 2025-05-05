using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class TriggerZoneActivator : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    
    [Header("Trigger Events")]
    [SerializeField] private UnityEvent onTriggerEnterEvent;
    [SerializeField] private UnityEvent onTriggerExitEvent;
    
    public bool IsPlayerInZone { get; private set; }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        IsPlayerInZone = true;
        onTriggerEnterEvent?.Invoke();
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        IsPlayerInZone = false;
        onTriggerExitEvent?.Invoke();
    }
}