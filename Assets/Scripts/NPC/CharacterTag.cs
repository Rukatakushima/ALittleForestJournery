using UnityEngine;

public class CharacterTag : MonoBehaviour
{
    [SerializeField] private string characterID;
    public string CharacterID => characterID;

    private void Start() => Register();

    private void OnEnable() => Register();

    private void OnDisable() => Unregister();

    private void Register()
    {
        if (CharacterRegistry.Instance == null || transform == null) return;
        CharacterRegistry.Instance.RegisterCharacter(CharacterID, transform);
    }

    private void Unregister()
    {
        if (CharacterRegistry.Instance == null) return;
        CharacterRegistry.Instance.UnregisterCharacter(CharacterID);
    }

}