using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public enum Scene { Village, Boat, Fishing, Ending, Interlude, Hut, Menu, Tutorial }

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

        if (currentScene == Scene.Hut || currentScene == Scene.Village || currentScene == Scene.Boat)
        {
            UIManager.instance.UpdateMoneyCount(money);
        }

        StartCoroutine(UIManager.instance.FadeIn(1f));
    }

    public void AddMoney(int increase)
    {
        money = Math.Clamp(money + increase, 0, 99999999);
        UIManager.instance.UpdateMoneyCount(money);
    }

    private void UpdateCurrentScene()
    {
        string scene = SceneManager.GetActiveScene().name;
        switch (scene)
        {
            case "EveningVillage":
            case "MorningVillage":
                currentScene = Scene.Village;
                break;
            case "Hut":
                currentScene = Scene.Hut;
                break;
            case "Fishing":
                currentScene = Scene.Fishing;
                break;
            case "MorningBoat":
            case "EveningBoat":
                currentScene = Scene.Boat;
                break;
            case "GoingToFish":
                currentScene = Scene.Interlude;
                break;
            case "MainMenu":
                currentScene = Scene.Menu;
                break;
            case "Tutorial":
                currentScene = Scene.Tutorial;
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
