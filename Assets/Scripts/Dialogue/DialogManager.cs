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

    private void Awake()
    {
        that = this;
    }

    IEnumerator TypeSentenceAndSetName(string name, string sentence, float speedLetters)
    {
        fieldName.text = name;// т.е. сокращение от fieldName.GetComponent<Text>().text
        RectTransform dialogBoxRect = fieldDialogBox.GetComponent<RectTransform>();

        if (GameObject.Find(name))
        {
            ////////////////////// ИЗМЕНЕНИЕ ПОЗИЦИИ ДИАЛОГА
            ////сейчас Pos Y устанавливается на то, что мы указываем в сцене. Мб из-за аниматора (когда окно диалога появляется)
            dialogBoxRect.anchoredPosition = new Vector2(110, 110);
            //Vector2 anchoredPosition;
            //RectTransformUtility.ScreenPointToLocalPointInRectangle(dialogBoxRect.parent.GetComponent<RectTransform>(), RectTransformUtility.WorldToScreenPoint(Camera.main, GameObject.Find(name).transform.position), null, out anchoredPosition);
        }
        else
        {
            dialogBoxRect.anchoredPosition = new Vector2(0, 130); // По умолчанию
        }

        fieldSentence.text = ""; //наш диалог между ""
        foreach (char letter in sentence.ToCharArray()) //для каждой буквы будем прибавлять след. букву
        {
            fieldSentence.text += letter;
            yield return new WaitForSeconds(Time.deltaTime * speedLetters);
        }
    }

    public static void EndDialogue()
    {
        that.boxAnim.SetBool("boxOpen", false);
    }

    public void StartDialogue(int idDialog)
    {
        that.boxAnim.SetBool("boxOpen", true);
        that.startAnim.SetBool("startOpen", false);

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