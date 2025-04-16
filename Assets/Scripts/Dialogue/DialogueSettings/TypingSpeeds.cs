using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DialogueSettings", menuName = "Dialogue/TypingSpeeds")]
public class TypingSpeeds : ScriptableObject
{
    public List<float> availableSpeeds = new List<float> { 0.5f, 1f, 1.5f, 2f };
}