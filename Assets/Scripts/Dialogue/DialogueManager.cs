using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static DialogueData;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    private string _dialogueName;
    private List<DialogueBranch> _dialogue;
    private List<DialogueNode> _nodes;
    private DialogueNode _currentNode;
    private int _currentNodeIndex;
    private int _currentSentenceIndex;
    
    private List<ChoiceNode> _choices;
    private int _currentChoiceIndex;

    public UnityEvent<string, string> onDialogueBranchStarted;
    public UnityEvent<DialogueNode, int> onDialogueNodeActive;
    public UnityEvent<DialogueNode, int> onChoiceNodeActive;
    public UnityEvent<string> onChoiceMade;
    public UnityEvent<string, string> onDialogueBranchEnded;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetDialogue(DialogueData dialogue)
    {
        _dialogue = dialogue.dialogue;
        _dialogueName = dialogue.DialogueName;
    }

    public void StartDialogueBranch(string branchID)
    {
        if (!TryGetDialogueBranch(branchID, out DialogueBranch branch)) return;
        
        CacheChoices(branch);

        StartDialogueNode(_nodes[_currentNodeIndex]);
        onDialogueBranchStarted?.Invoke(_dialogueName, branchID);
        
        branch.isRead = true;
    }

    private bool TryGetDialogueBranch(string branchID, out DialogueBranch dialogueBranch)
    {
        dialogueBranch = _dialogue.Find(d => d.ID == branchID);

        if (dialogueBranch is { dialogueNodes: { Count: > 0 } })
        {
            _nodes = dialogueBranch.dialogueNodes;
            _currentNodeIndex = 0;

            return !(_nodes[_currentNodeIndex].sentences.Count <= 0
            || _currentSentenceIndex > _nodes[_currentNodeIndex].sentences.Count);
        }
        else
        {
            EndDialogueNode();
            Debug.LogWarning($"Dialogue {branchID} is not found or empty or Sentence is out of array / null");
            return false;
        }
    }

    private void CacheChoices(DialogueBranch dialogueBranch)
    {
        _choices = new List<ChoiceNode>();
        foreach (var line in dialogueBranch.dialogueNodes)
        {
            if (line is ChoiceNode choiceLine)
                _choices.Add(choiceLine);
        }
        _currentChoiceIndex = 0;
    }

    private void StartDialogueNode(DialogueNode dialogueNode)
    {
        if (dialogueNode == null)
        {
            EndDialogueNode();
            Debug.LogWarning("DialogueNode is null ");
            return;
        }

        _currentNode = dialogueNode;
        _currentSentenceIndex = 0;

        if (dialogueNode is ChoiceNode choiceLine)
        {
            onChoiceNodeActive?.Invoke(choiceLine, _currentSentenceIndex);
        }
        else
        {
            onDialogueNodeActive?.Invoke(dialogueNode, _currentSentenceIndex);
        }
    }
    
    public void ShowNextChoiceLine()
    {
        if (++_currentChoiceIndex >= _choices.Count) return;

        StartDialogueNode(_choices[_currentChoiceIndex]);
    }

    public void ShowPreviousChoiceLine()
    {
        if (--_currentChoiceIndex < 0) return;
        
        StartDialogueNode(_choices[_currentChoiceIndex]);
    }


    public void NextDialogueLine()
    {
        if (_currentNode is ChoiceNode choiceLine)
        {
            StartDialogueBranch(choiceLine.nextDialogueBranchID);
            onChoiceMade?.Invoke(choiceLine.nextDialogueBranchID);
            return;
        }

        _currentSentenceIndex++;

        if (_currentNode != null && _currentSentenceIndex < _currentNode.sentences.Count)
        {
            // Next Sentence
            onDialogueNodeActive?.Invoke(_currentNode, _currentSentenceIndex);
        }
        else if (_currentNode == null || ++_currentNodeIndex >= _nodes.Count)
        {
            EndDialogueNode();
        }
        else
        {
            // Next Dialogue Line
            _currentSentenceIndex = 0;
            StartDialogueNode(_nodes[_currentNodeIndex]);
        }
    }

    public void EndDialogueNode()
    {
        onDialogueBranchEnded?.Invoke(_dialogueName, _dialogue[_currentNodeIndex].ID);
        
        _nodes = null;
        _currentNodeIndex = 0;
    }
}