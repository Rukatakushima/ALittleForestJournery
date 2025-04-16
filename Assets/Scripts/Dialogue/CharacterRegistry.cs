using System.Collections.Generic;
using UnityEngine;

public class CharacterRegistry : MonoBehaviour
{
    public static CharacterRegistry Instance { get; private set; }

    private readonly Dictionary<string, Transform> characters = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RegisterCharacter(string characterID, Transform transform)
    {
        if (!characters.ContainsKey(characterID))
            characters.Add(characterID, transform);
        else
            Debug.LogWarning($"Character {characterID} already registered!");
    }

    public void UnregisterCharacter(string characterID)
    {
        if (characters.ContainsKey(characterID))
            characters.Remove(characterID);
    }

    public Transform GetCharacterTransform(string characterID) => characters.TryGetValue(characterID, out var transform) ? transform : null;
}
