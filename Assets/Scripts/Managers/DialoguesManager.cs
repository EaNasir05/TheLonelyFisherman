using UnityEngine;
using Yarn.Unity;

public class DialoguesManager : MonoBehaviour
{
    public static DialoguesManager instance;
    private DialogueRunner dialogueRunner;
    public bool isTalking = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        dialogueRunner = gameObject.GetComponent<DialogueRunner>();

        dialogueRunner = FindFirstObjectByType<DialogueRunner>();
        if (dialogueRunner == null)
            Debug.LogError("DialogueRunner not found in the scene.");
        else
        {
            dialogueRunner.AddCommandHandler<string>("ChangeScene", GameManager.instance.ChangeScene);
        }
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
