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
    public Transform characterTransform;
    public List<Dialog> DialogsQueue;

    private void Awake()
    {
        that = this;
    }

    IEnumerator TypeSentenceAndSetName(string name, string sentence, float speedLetters)
    {
        fieldName.text = name;// = fieldName.GetComponent<Text>().text
        RectTransform dialogBoxRect = fieldDialogBox.GetComponent<RectTransform>();

        MoveDialogBox(name);

        fieldSentence.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            fieldSentence.text += letter;
            yield return new WaitForSeconds(Time.deltaTime * speedLetters);
        }
    }

    private void MoveDialogBox(string characterName)
    {
        bool shouldEnableAnimator = that.boxAnim.enabled;
        that.boxAnim.enabled = false;
        if (characterName == characterTransform.gameObject.name)
        {
            Camera camera = Camera.main;
            fieldDialogBox.transform.position = camera.WorldToScreenPoint(characterTransform.position) + new Vector3(0, 115f);
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