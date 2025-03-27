using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DialogueSettings", menuName = "Dialogue/Characters")]
public class DialogueSpeakers : ScriptableObject
{
    [System.Serializable]
    public class Speaker
    {
        public string Name;
        public Transform Transform;

        public override string ToString() => string.IsNullOrEmpty(Name) ? "Unnamed Speaker" : Name;
    }

    public List<Speaker> availableSpeakers = new List<Speaker>();
}