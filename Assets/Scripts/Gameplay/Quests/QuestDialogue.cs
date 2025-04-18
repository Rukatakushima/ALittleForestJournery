using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestDialogue : MonoBehaviour
{
    public Quest quest;
    public string questDialogueName;
    public int startDialogueID;
    public int[] progressDialogueIDs;
    public int completeDialogueID;
    public int afterwordsDialogueID;
    public int failDialogueID;

    private void Awake()
    {
        quest.onQuestStarted.AddListener(StartQuestDialogue);
        // quest.onQuestProgress.AddListener(CompleteQuestDialogue);
        quest.onQuestCompleted.AddListener(CompleteQuestDialogue);
    }

    public void StartQuestDialogue() => OpenQuestDialogue(startDialogueID);
    public void CompleteQuestDialogue() => OpenQuestDialogue(completeDialogueID);
    public void AfterwordsQuestDialogue() => OpenQuestDialogue(afterwordsDialogueID);
    public void FailQuestDialogue() => OpenQuestDialogue(failDialogueID);
    public void OpenQuestDialogue(int dialogueID) => DialogueManager.Instance.StartDialogue(/*questDialogueName,*/ dialogueID);
}
