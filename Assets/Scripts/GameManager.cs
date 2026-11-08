using UnityEngine;
using Yarn.Unity;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private InMemoryVariableStorage yarnVariableStorage;
    private DialogueRunner dialogueRunner;

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
        if (yarnVariableStorage == null)
        {
            yarnVariableStorage = FindFirstObjectByType<InMemoryVariableStorage>();
            if (yarnVariableStorage == null)
                Debug.LogError("InMemoryVariableStorage not found in the scene.");
            else
                DontDestroyOnLoad(yarnVariableStorage.gameObject);
        }

        dialogueRunner = FindFirstObjectByType<DialogueRunner>();
        if (dialogueRunner == null)
            Debug.LogError("DialogueRunner not found in the scene.");
        else
        {
            dialogueRunner.AddCommandHandler<string>("ChangeScene", ChangeScene);
        }
    }

    public void ChangeScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void PrintYarnVariable(string variableName)
    {
        if (yarnVariableStorage != null)
        {
            float value;
            if (yarnVariableStorage.TryGetValue(variableName, out value))
                Debug.Log($"Yarn Variable '{variableName}': {value}");
            else
                Debug.LogWarning($"Yarn Variable '{variableName}' not found.");
        }
    }
}
