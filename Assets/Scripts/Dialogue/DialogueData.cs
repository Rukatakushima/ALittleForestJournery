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
    [Header("Presets")]
    [SerializeField] private List<Speaker> speakers;
    [SerializeField] private List<float> typingSpeeds;
    public List<Speaker> AvailableSpeakers => speakers ?? new List<Speaker>();
    public List<float> AvailableSpeeds => typingSpeeds ?? new List<float> { 1f };

    [Header("Dialogue")]
    [SerializeField] private string dialogueName;
    public string DialogueName => dialogueName;
    public List<DialogueBranch> dialogue;

    [Serializable]
    public class Speaker
    {
        public string speakerName;
        public string speakerID;
    }

    [Serializable]
    public class DialogueBranch
    {
        [SerializeField] private string id;
        public string ID => id;
        public bool isRead;

        [SerializeReference]
        public List<DialogueNode> dialogueNodes;
    }

    [Serializable]
    public class DialogueNode
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
                        string.IsNullOrEmpty(speaker.speakerName) ?
                            speaker.speakerID :
                            $"{speaker.speakerName} ({speaker.speakerID})",
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

    [Serializable]
    public class ChoiceNode : DialogueNode
    {
        public string nextDialogueBranchID;
    }
}