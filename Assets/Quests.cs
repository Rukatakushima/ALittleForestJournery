using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quests : MonoBehaviour
{
    [HideInInspector]
    public int idPickedUp;
    public DialogManager dialogManager;
    public Animator startAnim;
    [HideInInspector]
    public int questNumber;
    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag != "Player")
        {
            idPickedUp = other.gameObject.GetComponent<Pickup>().id;
            
            if(other.tag != "Player" && other.gameObject.GetComponent<Pickup>()) 
            {
                questNumber++;
                Destroy(other.gameObject);

                switch(idPickedUp)
                {
                    case 0:
                    {
                        startAnim.SetBool("startOpen", true);
                        dialogManager.StartDialogue(1);
                    }
                    break;
                    
                    case 1:
                    {
                        startAnim.SetBool("startOpen", true);
                        dialogManager.StartDialogue(2);
                    }
                    break;

                    case 2:
                    {
                        startAnim.SetBool("startOpen", true);
                        dialogManager.StartDialogue(3);
                    }
                    break;

                    case 3:
                    {
                        startAnim.SetBool("startOpen", true);
                        dialogManager.StartDialogue(4);
                    }
                    break;

                    default:
                    break;

                }

                if(questNumber==4)
                {
                    dialogManager.StartDialogue(5);
                }
            }
        }
    }
}
