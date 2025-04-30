using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
[CreateAssetMenu(fileName = "DialogueData", menuName = "Dialogue/Dialogue")]
public class DialogueData : ScriptableObject
{
    [SerializeField] private DialogueSpeakers characters;
    [SerializeField] private TypingSpeeds typingSpeeds;
    [SerializeField] private string dialogueName;
    public string DialogueName => dialogueName;
    public List<Dialogue> dialogues;

    [Serializable]
    public class Dialogue
    {
        public int ID;
        public bool isRead = false;
        public List<DialogueLine> DialogueLines;
    }

    [Serializable]
    public class DialogueLine
    {
        [ValueDropdown("GetAvailableSpeakers")] public Speaker speaker;
        [ValueDropdown("GetAvailableSpeeds")] public float speed = 5f;
        [TextArea(1, 3)] public List<string> sentences = new();


#if UNITY_EDITOR
        private List<ValueDropdownItem> GetAvailableSpeakers()
        {
            var speakers = new List<ValueDropdownItem>();
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            var data = AssetDatabase.LoadAssetAtPath<DialogueData>(path);

            if (data?.AvailableSpeakers != null)
            {
                foreach (var speaker in data.AvailableSpeakers)
                {
                    speakers.Add(new ValueDropdownItem(
                        string.IsNullOrEmpty(speaker.Name) ?
                            speaker.SpeakerID :
                            $"{speaker.Name} ({speaker.SpeakerID})",
                        speaker));
                }
            }

            return speakers;
        }

        private List<float> GetAvailableSpeeds()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            var data = AssetDatabase.LoadAssetAtPath<DialogueData>(path);
            return data?.AvailableSpeeds ?? new List<float> { 1f };
        }
#endif
    }

    public List<Speaker> AvailableSpeakers => characters?.availableSpeakers ?? new List<Speaker>();
    public List<float> AvailableSpeeds => typingSpeeds?.availableSpeeds ?? new List<float> { 1f };
}