using UnityEngine;
using Yarn.Unity;

public class DialoguesManager : MonoBehaviour
{
    public static DialoguesManager instance;
    public DialogueRunner dialogueRunner;
    public bool isTalking = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //dialogueRunner = FindFirstObjectByType<DialogueRunner>();
    }

    public void StartDialogue(string nodeName)
    {
        if (dialogueRunner != null)
        {
            isTalking = true;
            dialogueRunner.StartDialogue(nodeName);
        }
        else
        {
            Debug.LogError("DialogueRunner not found in the scene.");
        }
    }

    public void NextLine()
    {
        if (dialogueRunner != null && isTalking)
        {
            dialogueRunner.RequestNextLine();
        }
        else
        {
            Debug.LogError("DialogueRunner not found in the scene.");
        }
    }
}
