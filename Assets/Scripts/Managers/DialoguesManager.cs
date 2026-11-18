using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class DialoguesManager : MonoBehaviour
{
    public static DialoguesManager instance;
    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] private LinePresenter linePresenter;
    public bool isTalking = false;
    private bool commandsReady = false;
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
        //Initialize();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene arg0, LoadSceneMode arg1)
    {
        Initialize();
    }

    private void Initialize()
    {
        if (dialogueRunner == null)
            Debug.LogError("DialogueRunner not found.");
        else if (!commandsReady)
        {
            dialogueRunner.AddCommandHandler<string>("ChangeScene", GameManager.instance.ChangeScene);
            dialogueRunner.AddCommandHandler("FadeIn", GameManager.instance.FadeIn);
            dialogueRunner.AddCommandHandler("NextDay", GameManager.instance.NextDay);
            dialogueRunner.AddCommandHandler("GoFishing", GameManager.instance.GoFishing);
            dialogueRunner.AddCommandHandler("BuildBoat", GameManager.instance.BuildBoat);
            dialogueRunner.AddCommandHandler("CloseShopUnlocked", UIManager.instance.ShowCloseShopButton);
            dialogueRunner.onDialogueComplete.AddListener(DialogueCompleted);
            commandsReady = true;
        }
    }

    public void StartDialogue(string nodeName)
    {
        if (!isTalking)
        {
            if (dialogueRunner != null)
            {
                isTalking = true;
                if (GameManager.instance.tutorialState == 1 || GameManager.instance.tutorialState == 4)
                {
                    linePresenter.useFadeEffect = false;
                    Time.timeScale = 0;
                }
                onDialogueActivation?.Invoke(true);
                dialogueRunner.StartDialogue(nodeName);
            }
            else
            {
                Debug.LogError("DialogueRunner not found in the scene.");
            }
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
        onDialogueActivation?.Invoke(false);
        isTalking = false;
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            linePresenter.useFadeEffect = false;
        }
    }
}
