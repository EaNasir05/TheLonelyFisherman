using System;
using UnityEngine;
using Yarn.Unity;

public class DialoguesManager : MonoBehaviour
{
    public static DialoguesManager instance;
    private DialogueRunner dialogueRunner;
    public bool isTalking = false;
    public Action<bool> onDialogueActivation;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
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
            dialogueRunner.onDialogueComplete.AddListener(DialogueCompleted);
        }
    }

    public void StartDialogue(string nodeName)
    {
        if (!isTalking)
        {
            if (dialogueRunner != null)
            {
                isTalking = true;
                onDialogueActivation.Invoke(true);
                dialogueRunner.StartDialogue(nodeName);
            }
            else
            {
                Debug.LogError("DialogueRunner not found in the scene.");
            }
        }
        else
        {
            Debug.LogError("There's already a dialogue active");
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

    public void DialogueCompleted()
    {
        onDialogueActivation.Invoke(false);
        isTalking = false;
    }
}
