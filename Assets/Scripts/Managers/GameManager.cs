using UnityEngine;
using Yarn.Unity;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private InMemoryVariableStorage yarnVariableStorage;
    public bool isPaused = false;

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

    public void SetYarnVariable(string variableName, float value)
    {
        if (yarnVariableStorage != null)
        {
            yarnVariableStorage.SetValue(variableName, value);
        }
    }

    public void SetYarnVariable(string variableName, int value)
    {
        if (yarnVariableStorage != null)
        {
            yarnVariableStorage.SetValue(variableName, value);
        }
    }

    public void SetYarnVariable(string variableName, bool value)
    {
        if (yarnVariableStorage != null)
        {
            yarnVariableStorage.SetValue(variableName, value);
        }
    }

    public void SetYarnVariable(string variableName, string value)
    {
        if (yarnVariableStorage != null)
        {
            yarnVariableStorage.SetValue(variableName, value);
        }
    }
}
