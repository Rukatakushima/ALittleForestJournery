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
        public string CharacterName;
        public bool NameAtLeft;
        public string Sentence;
        public Sprite Sprite;
        public float SpeedLetters;
    }

    public Text fieldName;
    public Text fieldSentence;
    public Sprite fieldSprite;

    public List<Dialog> DialogList;
}
