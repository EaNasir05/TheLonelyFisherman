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
    public bool tutorial = true;
    private int day = 0;
    private int money = 0;
    [SerializeField] private bool grandsonHere = false;
    [SerializeField] private UpgradesList upgradesList;

    public bool IsGrandsonHere() => grandsonHere;
    public Scene GetCurrentScene() => currentScene;
    public int GetDay() => day;
    public int GetMoney() => money;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            upgradesList.ResetUpgrades();
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
            FishingManager.instance.Initialize();
        }

        StartCoroutine(UIManager.instance.FadeIn(1f));
    }

    public void AddMoney(int increase)
    {
        money = Math.Clamp(money + increase, 0, 999999999);
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
            case "MainMenu":
                currentScene = Scene.Menu;
                break;
            default:
                currentScene = Scene.Ending;
                break;
        }
    }

    public void ChangeScene(string sceneName)
    {
        StartCoroutine(UIManager.instance.FadeOut(1f, sceneName));
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
