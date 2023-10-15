using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public Text dialogueText;
    public Text nameText;

    public Animator boxAnim;
    public Animator startAnim;

    private Queue<string> sentences;

    public int sentencesNumber = 0;
    List<int> nameBlait = new List<int> {1, 3};
    List<int> nameTox = new List<int> {2, 4, 5};
    float speedLetters = 5f;

    public Queue<string> dialogue1 = new Queue<string>();
    private void Start()
    {

        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        boxAnim.SetBool("boxOpen", true);
        startAnim.SetBool("startOpen", false);

        //устанавливаем имя из inspector
        //nameText.text = dialogue.name; 

        sentences.Clear();
        sentences.Enqueue("This is the first sentence");
        sentences.Enqueue("This is the second sentence");
        sentences.Enqueue("This is the third sentence");
        sentences.Enqueue("This is the 4rd sentence");

        dialogue1.Clear();
        dialogue1.Enqueue("Wow! Wow! Wow!");
        dialogue1.Enqueue("Look at the hat!");
        dialogue1.Enqueue("It is amazing!");
        dialogue1.Enqueue("I'll keep it close");

        // foreach(string sentence in dialogue.sentences)
        // {
        //     sentences.Enqueue(sentence);
        // }

        DisplayNextSentence();
    }
    public void DisplayNextSentence()
    {
        sentencesNumber++;
        if(sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        // УСТАНОВКА ИМЕНИ //

        foreach(int num in nameBlait)
        {
            if (sentencesNumber == num)
            {
                nameText.text = "Blait";
                boxAnim.SetBool("namePosChange", true);
                break;
            }
        }

        foreach(int num in nameTox)
        {
            if (sentencesNumber == num)
            {
                nameText.text = "Tox";
                boxAnim.SetBool("namePosChange", false);
                break;

            }
        }
        
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = ""; //наш диалог между ""
        foreach(char letter in sentence.ToCharArray()) //для каждой буквы будем прибавлять след. букву
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(Time.deltaTime * speedLetters);
        }
    }

    public void EndDialogue()
    {
        boxAnim.SetBool("boxOpen", false);
    }

}