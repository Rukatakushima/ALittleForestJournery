using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// ЗДЕСЬ БУДУТ ВСЕ ФУНКЦИИ ДЛЯ ДИАЛОГОВ: ВЫЗОВ, ЗАКРЫТИЕ, НАПИСАНИЕ, ПЕРЕМЕЩЕНИЕ, ИЗМЕНЕНИЯ И ТД
public class DialogManager : MonoBehaviour
{
    public static DialogManager that;
    #region Parameters

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
    public Text fieldName, fieldSentence; // для теста перемещения диалога
    public Transform characterTransform;
    public Transform mainCharacterTransform;
    public List<Dialog> DialogsQueue;

    #endregion Parameters
    private void Awake()
    {
        that = this;
    }
    /*
    #region Quests
    public bool[] questStates;// Список булевых переменных для проверки квестов
    public bool CheckQuestState(int questId)
    {
        return questStates[questId];
    }
    public void SetQuestState(int questId, bool state)
    {
        questStates[questId] = state;
    }
    #endregion
    */
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
    IEnumerator TypeSentenceAndSetName(string name, string sentence, float speedLetters)
    {
        fieldName.text = name;

        that.boxAnim.enabled = false;
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
        Transform targetTransform = null;
        if (characterName == characterTransform.gameObject.name)
        {
            targetTransform = characterTransform;
        }
        else if (characterName == mainCharacterTransform.gameObject.name)
        {
            targetTransform = mainCharacterTransform;
        }
        if (characterName == targetTransform.gameObject.name)
        {
            Camera camera = Camera.main;
            fieldDialogBox.transform.position = camera.WorldToScreenPoint(targetTransform.position) + new Vector3(0, 115f);
        }
        else
        {
            fieldDialogBox.transform.position = new Vector2(0, 1.5f);
        }
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
    public static void EndDialogue()
    {
        that.boxAnim.enabled = true;
        that.boxAnim.SetBool("boxOpen", false);
    }
}