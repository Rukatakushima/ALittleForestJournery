using UnityEngine;
using UnityEngine.Events;

public abstract class BaseLevelSpawner<TLevelData> : MonoBehaviour
    where TLevelData : ScriptableObject
{
    [SerializeField] protected TLevelData level;
    public UnityEvent OnLevelSpawned = new();
    
    public abstract void SpawnLevel();
}