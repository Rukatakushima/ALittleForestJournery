using UnityEngine;

public class QuestDialogue : MonoBehaviour
{
    public Quest quest;
    public string questDialogueName;
    public string startDialogueID;
    public string[] progressDialogueIDs;
    public string completeDialogueID;
    public string afterwordsDialogueID;
    public string failDialogueID;

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
    public void OpenQuestDialogue(string dialogueID) => DialogueManager.Instance.StartDialogue(/*questDialogueName,*/ dialogueID);
}
