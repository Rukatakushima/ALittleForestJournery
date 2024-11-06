using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//ЗДЕСЬ БУДУТ ПРОВЕРКИ (ФУНКЦИИ В DialogManager) И ВЫЗОВ НЕОБХОДИМОГО ДИАЛОГА (где-то должен быть скрипт со всеми диалогами)
public class DialogTrigger : MonoBehaviour
{
    [HideInInspector]
    public int idPickedUp;
    //public DialogManager dialogManager;
    [HideInInspector]
    public int questNumber;
    public Animator startAnim;
    #region Check Quests and Start Dialog
    /*
    public int dialogId;
    public int requiredQuestId; // ID квеста, необходимого для начала диалога
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (DialogManager.that.CheckQuestState(requiredQuestId))
            {
                startAnim.SetBool("startOpen", true);
            }
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        startAnim.SetBool("startOpen", false);
        DialogManager.EndDialogue();
    }

    public void StartDialogue()
    {
        DialogManager.that.StartDialogue(dialogId);
    }
    */
    #endregion
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            startAnim.SetBool("startOpen", true);
        }
        else if (other.gameObject.GetComponent<Pickup>())
        {
            idPickedUp = other.gameObject.GetComponent<Pickup>().id;
            questNumber++;
            Destroy(other.gameObject);
            if (questNumber == 4)
            {
                DialogManager.that.StartDialogue(5);
            }
            else
            {
                startAnim.SetBool("startOpen", true);
                DialogManager.that.StartDialogue(idPickedUp + 1);
            }
        }
    }
    public void OnTriggerExit2D(Collider2D other)
    {
        startAnim.SetBool("startOpen", false);
        DialogManager.EndDialogue();
    }
}