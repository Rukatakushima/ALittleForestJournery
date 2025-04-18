using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "DialogueData", menuName = "Dialogue/Dialogue")]
public class DialogueData : ScriptableObject
{
    [SerializeField] private DialogueSpeakers Characters;
    [SerializeField] private TypingSpeeds TypingSpeeds;
    [SerializeField] private string dialogueName;
    public string DialogueName => dialogueName;
    public List<Dialogue> dialogues;

    [System.Serializable]
    public class Dialogue
    {
        public int ID;
        public bool IsRead = false;
        public List<DialogueLine> DialogueLines;
    }

    [System.Serializable]
    public class DialogueLine
    {
        [ValueDropdown("GetAvailableSpeakers")] public Speaker speaker;
        [ValueDropdown("GetAvailableSpeeds")] public float Speed = 5f;
        [TextArea(1, 3)] public List<string> Sentences = new();


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

    public List<Speaker> AvailableSpeakers => Characters?.availableSpeakers ?? new List<Speaker>();
    public List<float> AvailableSpeeds => TypingSpeeds?.availableSpeeds ?? new List<float> { 1f };
}