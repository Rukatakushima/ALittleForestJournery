using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    private static DialogManager instance;

    [System.Serializable]
    public class Dialog
    {
        public int id;
        public bool isRead;
        public List<Sentence> SentencesList;
    }
    
    [System.Serializable]
    public class Sentence
    {
        public string CharacterName;
        public string Letters;
        public float SpeedLetters;
    }

    [HideInInspector] public int count;
    [HideInInspector] public int currentIndex;
    [HideInInspector] public List<Sentence> CurrentSentences;

    [SerializeField] private Animator startDialogueButtonAnim;
    [SerializeField] private Animator dialogueBoxAnim;
    [SerializeField] private GameObject dialogBoxGameObject;
    [SerializeField] private Text fieldName;
    [SerializeField] private Text fieldSentence;
    [SerializeField] private Transform characterTransform;
    [SerializeField] private List<Dialog> DialogsQueue;
    
    public static DialogManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.Log("DialogManager is null! Make sure it is in the scene.");
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void SetStartDialogueButtonAnimation(bool isOpen)
    {
        if (startDialogueButtonAnim != null)
        {
            startDialogueButtonAnim.SetBool("startDialougueButtonOpen", isOpen);
        }
    }
    public void SetStartDialogueAnimation(bool isOpen)
    {
        if (dialogueBoxAnim != null)
        {
            dialogueBoxAnim.SetBool("dialogueBoxOpen", isOpen);
        }
    }

    public void StartDialogue(int idDialog)
    {
        SetStartDialogueButtonAnimation(false);
        SetStartDialogueAnimation(true);
        // dialogueBoxAnim.enabled = false;

        Dialog currentDialog = DialogsQueue.Find(x => x.id == idDialog);
        if (currentDialog != null)
        {
            CurrentSentences = currentDialog.SentencesList;
            count = CurrentSentences.Count;
            currentIndex = 0;

            StartCoroutine(TypeSentenceAndSetName(CurrentSentences[0]));
        }
        else
        {
            Debug.LogWarning($"Dialog with ID {idDialog} not found.");
            // DIALOG NOT FOUND HELP FUNCTION
        }
    }

    private IEnumerator TypeSentenceAndSetName(Sentence sentence)
    {
        fieldName.text = sentence.CharacterName;
        fieldSentence.text = "";

        SetDialogueBoxPosition(sentence.CharacterName);

        foreach (char letter in sentence.Letters.ToCharArray())
        {
            fieldSentence.text += letter;
            yield return new WaitForSeconds(Time.deltaTime * sentence.SpeedLetters);
        }
    }

    private void SetDialogueBoxPosition(string characterName)
    {
        RectTransform dialogBoxRect = dialogBoxGameObject.GetComponent<RectTransform>();

        if (characterName == characterTransform.name)
        {
            Camera camera = Camera.main;
            dialogBoxGameObject.transform.position = camera.WorldToScreenPoint(characterTransform.position) + new Vector3(0, 115f);
            // if ()
            // {
// // установить диалог. окно рядом с границей экрана, чтобы полностью помещалось, а не уходило за экран
            // }
        }
        else
        {
            dialogBoxRect.anchoredPosition = new Vector2(0, 1.5f);
        }
    }

    public void DisplayNextSentence()
    {
        currentIndex++;
        StopAllCoroutines();

        if (currentIndex < count)
        {
            StartCoroutine(TypeSentenceAndSetName(CurrentSentences[currentIndex]));
        }
        else
        {
            EndDialogue();
        }
    }

    public static void EndDialogue()
    {
        if (Instance.dialogueBoxAnim != null)
        {
            Instance.dialogueBoxAnim.SetBool("dialogueBoxOpen", false);
            Instance.dialogueBoxAnim.enabled = true;
        }
    }
}
