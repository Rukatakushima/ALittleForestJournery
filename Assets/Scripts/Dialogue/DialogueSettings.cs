using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DialogueSettings", menuName = "Dialogue/Characters")]
public class DialogueSpeakers : ScriptableObject
{
    [System.Serializable]
    public class Speaker
    {
        public string Name;
        public string CharacterID;
    }

    public List<Speaker> availableSpeakers = new List<Speaker>();
}