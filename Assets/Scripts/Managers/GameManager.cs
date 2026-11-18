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
    public int tutorialState = 0;
    private int day = 0;
    private int boatLevel = 0;
    private bool builtBoat = false;
    [SerializeField] private int[] boatCosts;
    private int money = 10;
    [SerializeField] private bool grandsonHere = false;
    [SerializeField] private UpgradesList upgradesList;

    public bool IsGrandsonHere() => grandsonHere;
    public Scene GetCurrentScene() => currentScene;
    public int GetDay() => day;
    public int GetMoney() => money;
    public int GetBoatLevel() => boatLevel;

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
        if (yarnVariableStorage == null)
        {
            yarnVariableStorage = FindFirstObjectByType<InMemoryVariableStorage>();
            if (yarnVariableStorage == null)
                Debug.LogError("InMemoryVariableStorage not found in the scene.");
            else
            {
                DontDestroyOnLoad(yarnVariableStorage.gameObject);
                SetYarnVariable("$boatCost1", boatCosts[0]);
                SetYarnVariable("$boatCost2", boatCosts[1]);
                SetYarnVariable("$boatCost3", boatCosts[2]);
                SetYarnVariable("$boatCost4", boatCosts[3]);
                SetYarnVariable("$boatCost5", boatCosts[4]);
            }
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
                if (previousScene == Scene.Village)
                {
                    PlayerSpawner.instance.SpawnPlayerFromVillage();
                    UIManager.instance.onFadeIn += StartMorningDialogue;
                }
                else if (previousScene == Scene.Boat)
                {
                    PlayerSpawner.instance.SpawnPlayerFromBoat();
                    if (builtBoat)
                    {
                        builtBoat = false;
                        UIManager.instance.onFadeIn += StartBoatDialogue;
                    }
                }
                else if (previousScene == Scene.Fishing)
                {
                    PlayerSpawner.instance.SpawnPlayerFromFishing();
                    UIManager.instance.onFadeIn += StartEveningDialogue;
                }
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
            case "Boat":
                currentScene = Scene.Boat;
                UIManager.instance.UpdateMoneyCount(money);
                bool upgradable = money >= boatCosts[boatLevel];
                SetYarnVariable("$boatUpgradable", upgradable);
                SetYarnVariable("$boatCost", boatCosts[boatLevel]);
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
                FishingManager.instance.Initialize();
                StartCoroutine(StartPrologueDialogue());
                break;

            default:
                currentScene = Scene.Ending;
                //Fai partire il dialogo finale
                break;
        }
        Debug.Log("Setup scena " + currentScene);
    }

    public void ChangeScene(string sceneName)
    {
        StartCoroutine(UIManager.instance.FadeOut(1f, sceneName));
    }

    public void SkipIntro()
    {
        tutorialState = 3;
        currentScene = Scene.Fishing;
        ChangeScene("EveningVillage");
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
        builtBoat = true;
        boatLevel++;
        SetYarnVariable("$boatLevel", boatLevel);
        if (boatLevel == 5)
        {
            SetYarnVariable("$boatBuilt", true);
        }
        ChangeScene("EveningVillage");
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

    private void StartMorningDialogue()
    {
        DialoguesManager.instance.StartDialogue("Morning");
    }

    private void StartEveningDialogue()
    {
        DialoguesManager.instance.StartDialogue("Back");
    }

    private void StartInterludeDialogue()
    {
        DialoguesManager.instance.StartDialogue("Go");
    }

    private IEnumerator StartPrologueDialogue()
    {
        yield return new WaitForSeconds(1f);
        DialoguesManager.instance.StartDialogue("Prologo");
    }

    private void StartBoatDialogue()
    {
        DialoguesManager.instance.StartDialogue("BoatBuild");
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
