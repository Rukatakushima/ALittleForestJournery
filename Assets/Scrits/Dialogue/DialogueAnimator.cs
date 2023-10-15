using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//появления диалога тогда, когда персонаж находится рядом с NPC

public class DialogueAnimator : MonoBehaviour
{
    public Animator startAnim;
    public DialogueManager dm;
    public Queue<string> dialogue1 = new Queue<string>();

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            startAnim.SetBool("startOpen", true);
        }
    }
     public void OnTriggerExit2D(Collider2D other)
    {
        startAnim.SetBool("startOpen", false);
        dm.EndDialogue();
    }
}
