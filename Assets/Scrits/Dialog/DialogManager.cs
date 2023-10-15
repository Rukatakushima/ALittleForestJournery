using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogManager : MonoBehaviour
{    
    [System.Serializable]
    public class Dialog
    {
        public Text CharacterName;
        public Text Sentence;
        public Texture2D Sprite;
        public float SpeedLetters;
    }

    public List<Dialog> DialogList;
}
