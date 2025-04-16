using UnityEngine;

public class CharacterTag : MonoBehaviour
{
    [SerializeField] private string characterID;
    public string CharacterID => characterID;

    private void OnEnable()
    {
        if (CharacterRegistry.Instance != null)
        {
            CharacterRegistry.Instance.RegisterCharacter(characterID, transform);
        }
    }

    private void OnDisable()
    {
        if (CharacterRegistry.Instance != null)
        {
            CharacterRegistry.Instance.UnregisterCharacter(characterID);
        }
    }
}