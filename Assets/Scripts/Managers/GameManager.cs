using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private Scene currentScene;
    private InMemoryVariableStorage yarnVariableStorage;
    public bool isPaused = false;
    [SerializeField] private bool grandsonHere = false;

    public bool IsGrandsonHere() => grandsonHere;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
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
        UpdateCurrentScene();

        if (yarnVariableStorage == null)
        {
            yarnVariableStorage = FindFirstObjectByType<InMemoryVariableStorage>();
            if (yarnVariableStorage == null)
                Debug.LogError("InMemoryVariableStorage not found in the scene.");
            else
                DontDestroyOnLoad(yarnVariableStorage.gameObject);
        }

        if (currentScene == Scene.Fishing)
        {
            //Settaggio variabili FishingManager e Hook
            FishingManager.instance.Initialize();
        }
    }

    private void UpdateCurrentScene()
    {
        string scene = SceneManager.GetActiveScene().name;
        switch (scene)
        {
            case "EveningIsland":
            case "MorningIsland":
                currentScene = Scene.Island;
                break;
            case "Hut":
                currentScene = Scene.Hut;
                break;
            case "Fishing":
                currentScene = Scene.Fishing;
                break;
            case "MorningOnBoat":
            case "EveningOnBoat":
                currentScene = Scene.Boat;
                break;
            default:
                currentScene = Scene.Ending;
                break;
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
