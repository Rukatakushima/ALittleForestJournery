using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    static DialogManager that;

    [System.Serializable]
    public class Sentence
    {
        public string CharacterName;
        public string Letters;
        public float SpeedLetters;
    }

    [System.Serializable]
    public class Dialog
    {
        public int id;
        public bool isRead;
        public List<Sentence> SentencesList;
    }

    [HideInInspector]
    public int count;
    [HideInInspector]
    public int currentIndex;
    [HideInInspector]
    public List<Sentence> CurrentSentences;
    public Animator startAnim;
    public Animator boxAnim;
    public GameObject fieldDialogBox, fieldNameBox; //для перемещения имени и спрайта
    public Text fieldName, fieldSentence;
    public List<Dialog> DialogsQueue;
    public Canvas canvas;

    private void Awake()
    {
        that = this;
    }

    // private void Update() 
    // {
    //ИЗ TypeSentenceAndSetName, чтобы окно следовало за объектом
    //     MoveDialogBox(name);
    // }

    IEnumerator TypeSentenceAndSetName(string name, string sentence, float speedLetters)
    {
        fieldName.text = name;// т.е. сокращение от fieldName.GetComponent<Text>().text
        RectTransform dialogBoxRect = fieldDialogBox.GetComponent<RectTransform>();

        MoveDialogBox(name);

        fieldSentence.text = ""; //наш диалог между ""
        foreach (char letter in sentence.ToCharArray()) //для каждой буквы будем прибавлять след. букву
        {
            fieldSentence.text += letter;
            yield return new WaitForSeconds(Time.deltaTime * speedLetters);
        }
    }

    private void MoveDialogBox(string characterName)
    {
        bool shouldEnableAnimator = that.boxAnim.enabled;
        that.boxAnim.enabled = false;
        if (GameObject.Find(characterName) is GameObject targetObject)
        {
            // Vector2 vector2 = RectTransformUtility.WorldToScreenPoint(Camera.main, targetObject.transform.position) + new Vector2(-400,0);
            // fieldDialogBox.GetComponent<RectTransform>().anchoredPosition = vector2;

            fieldDialogBox.GetComponent<RectTransform>().anchoredPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, targetObject.transform.position);

            /*
            // Получаем RectTransform объекта, который мы хотим позиционировать
            RectTransform fieldDialogBoxRect = fieldDialogBox.GetComponent<RectTransform>();

            // Получаем экранные координаты целевого объекта 
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, targetObject.transform.position);

            // Переводим экранные координаты в локальные координаты RectTransform 
            Vector2 anchoredPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(fieldDialogBoxRect.parent as RectTransform, screenPoint, Camera.main, out anchoredPosition);

            // Устанавливаем значения для RectTransform
            fieldDialogBoxRect.anchoredPosition = anchoredPosition;
*/
        }
        else
        {
            fieldDialogBox.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 1.5f);
        }
        if (shouldEnableAnimator)
        {
            that.boxAnim.enabled = true;
        }
    }
    public static void EndDialogue()
    {
        that.boxAnim.enabled = true;
        that.boxAnim.SetBool("boxOpen", false);
    }

    public void StartDialogue(int idDialog)
    {
        that.boxAnim.SetBool("boxOpen", true);
        that.startAnim.SetBool("startOpen", false);

        that.boxAnim.enabled = false;

        Dialog currentDialog = that.DialogsQueue.Find(x => x.id == idDialog);
        that.CurrentSentences = currentDialog.SentencesList;

        that.count = currentDialog.SentencesList.Count;
        that.currentIndex = 0;

        that.StartCoroutine(that.TypeSentenceAndSetName(that.CurrentSentences[0].CharacterName, that.CurrentSentences[0].Letters, that.CurrentSentences[0].SpeedLetters));

        //currentDialog.isRead = "true";
    }

    public void DisplayNextSentence()
    {
        currentIndex++;
        StopAllCoroutines();
        if (that.currentIndex < that.count)
        {
            that.StartCoroutine(that.TypeSentenceAndSetName(that.CurrentSentences[that.currentIndex].CharacterName, that.CurrentSentences[that.currentIndex].Letters, that.CurrentSentences[that.currentIndex].SpeedLetters));
        }

        else EndDialogue();
    }
}