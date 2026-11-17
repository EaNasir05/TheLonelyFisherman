using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public enum Scene { Village, Boat, Fishing, Ending, Interlude, Hut, Menu, Tutorial }

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private Scene previousScene;
    private Scene currentScene = Scene.Menu;
    private InMemoryVariableStorage yarnVariableStorage;
    public bool isPaused = false;
    public bool tutorial = true;
    private int day = 1;
    private int boatLevel = 0;
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
        if (yarnVariableStorage == null)
        {
            yarnVariableStorage = FindFirstObjectByType<InMemoryVariableStorage>();
            if (yarnVariableStorage == null)
                Debug.LogError("InMemoryVariableStorage not found in the scene.");
            else
                DontDestroyOnLoad(yarnVariableStorage.gameObject);
        }

        UpdateCurrentScene();
        if (currentScene != Scene.Menu && currentScene != Scene.Tutorial)
            FadeIn();
    }

    private void UpdateCurrentScene()
    {
        string scene = SceneManager.GetActiveScene().name;
        previousScene = currentScene;
        switch (scene)
        {
            case "EveningVillage":
            case "MorningVillage":
                currentScene = Scene.Village;

                UIManager.instance.UpdateMoneyCount(money);
                if (previousScene == Scene.Village || previousScene == Scene.Menu)
                    PlayerSpawner.instance.SpawnPlayerFromVillage();
                else if (previousScene == Scene.Boat)
                    PlayerSpawner.instance.SpawnPlayerFromBoat();
                else if (previousScene == Scene.Fishing)
                    PlayerSpawner.instance.SpawnPlayerFromFishing();
                
                if (previousScene == Scene.Fishing)
                    UIManager.instance.onFadeIn += StartEveningDialogue;
                break;

            case "Hut":
                currentScene = Scene.Hut;
                UIManager.instance.UpdateMoneyCount(money);
                break;

            case "Fishing":
                currentScene = Scene.Fishing;
                FishingManager.instance.Initialize();
                break;

            case "MorningBoat":
            case "EveningBoat":
                currentScene = Scene.Boat;

                UIManager.instance.UpdateMoneyCount(money);
                if (previousScene == Scene.Village || previousScene == Scene.Menu)
                    PlayerSpawner.instance.SpawnPlayerFromVillage();
                else if (previousScene == Scene.Boat)
                    PlayerSpawner.instance.SpawnPlayerFromBoat();

                if (previousScene == Scene.Boat)
                    UIManager.instance.onFadeIn += StartBoatDialogue;
                break;

            case "GoingToFish":
                currentScene = Scene.Interlude;
                UIManager.instance.onFadeIn += StartInterludeDialogue;
                break;

            case "MainMenu":
                currentScene = Scene.Menu;
                break;

            case "Tutorial":
                currentScene = Scene.Tutorial;
                UIManager.instance.onFadeIn += StartTutorialDialogue;
                StartCoroutine(StartPrologueDialogue());
                break;

            default:
                currentScene = Scene.Ending;
                //Fai partire il dialogo finale
                break;
        }
        Debug.Log("Setup scena " + currentScene);
        PrintYarnVariable("$visitIndex");
    }

    public void ChangeScene(string sceneName)
    {
        StartCoroutine(UIManager.instance.FadeOut(1f, sceneName));
    }

    public void FadeIn()
    {
        StartCoroutine(UIManager.instance.FadeIn(1f));
    }

    public void NextDay()
    {
        day++;
        SetYarnVariable("$dayIndex", day);
        UpdateGrandsonHere();
        ChangeScene("MorningVillage");
    }

    public void GoFishing()
    {
        if (grandsonHere)
            ChangeScene("GoingToFish");
        else
            ChangeScene("Fishing");
    }

    public void BuildBoat()
    {
        boatLevel++;
        ChangeScene("EveningBoat");
    }

    private void UpdateGrandsonHere()
    {
        int nextVisitDay = -1;
        yarnVariableStorage.TryGetValue("$visitIndex", out nextVisitDay);
        if (nextVisitDay == day)
            grandsonHere = true;
        else
            grandsonHere = false;
    }

    #region Economy

    public void AddMoney(int increase)
    {
        money = Math.Clamp(money + increase, 0, 99999999);
        UIManager.instance.UpdateMoneyCount(money);
    }

    #endregion Economy

    #region Dialogues

    private void StarMorningDialogue()
    {
        
    }

    private void StartEveningDialogue()
    {
        
    }

    private void StartInterludeDialogue()
    {
        
    }

    private void StartTutorialDialogue()
    {
        
    }

    private IEnumerator StartPrologueDialogue()
    {
        return null;
    }

    private void StartBoatDialogue()
    {
        
    }

    #endregion Dialogues

    #region Yarn Spinner

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

    #endregion Yarn Spinner
}
