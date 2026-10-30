using UnityEngine;
using Yarn.Unity;

public class DialoguesManager : MonoBehaviour
{
    public static DialoguesManager instance;
    private DialogueRunner dialogueRunner;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        dialogueRunner = FindFirstObjectByType<DialogueRunner>();
    }

    public void StartDialogue(string nodeName)
    {
        if (dialogueRunner != null)
        {
            dialogueRunner.StartDialogue(nodeName);
        }
        else
        {
            Debug.LogError("DialogueRunner not found in the scene.");
        }
    }
}
