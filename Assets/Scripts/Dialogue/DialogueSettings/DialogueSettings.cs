using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DialogueSettings", menuName = "Dialogue/Characters")]
public class DialogueSpeakers : ScriptableObject
{
    public List<Speaker> availableSpeakers = new List<Speaker>();
}

[System.Serializable]
public class Speaker
{
    public string Name;
    public CharacterTag characterTag;
    public string SpeakerID => characterTag.CharacterID;

    // public override string ToString()
    // {
    //     if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(CharacterID))
    //         return $"{Name} ({CharacterID})";
    //     return !string.IsNullOrEmpty(Name) ? Name : CharacterID;
    // }
}