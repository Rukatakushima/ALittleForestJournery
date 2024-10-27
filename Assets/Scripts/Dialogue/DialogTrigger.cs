using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
   public Animator startAnim;
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
        DialogManager.EndDialogue();
    }
}
