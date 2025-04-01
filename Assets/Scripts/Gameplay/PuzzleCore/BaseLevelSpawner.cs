using UnityEngine;
using UnityEngine.Events;

public abstract class BaseLevelSpawner<TLevelData> : MonoBehaviour
    where TLevelData : ScriptableObject
{
    public UnityEvent OnLevelSpawned = new();
    [SerializeField] protected TLevelData level;
    
    public abstract void SpawnLevel();
}