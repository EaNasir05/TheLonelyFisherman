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
    private int affection = 0;
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
    public int GetAffection() => affection;

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
            // Copia duplicata: disattivala subito, altrimenti riceve comunque sceneLoaded
            // e riesegue l'inizializzazione della scena prima di essere distrutta.
            enabled = false;
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
        if (instance != this)
            return;
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

    #region Scene Transition

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
                if (grandsonHere)
                    UIManager.instance.ShowGrandson();
                break;

            case "Hut":
                currentScene = Scene.Hut;
                UIManager.instance.UpdateMoneyCount(money);
                break;

            case "Fishing":
                currentScene = Scene.Fishing;
                if (grandsonHere)
                    UIManager.instance.ShowGrandson();
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
                UIManager.instance.UpdateBoatSprite(boatLevel);
                break;

            case "GoingToFish":
                currentScene = Scene.Interlude;
                UIManager.instance.onFadeIn += StartInterludeDialogue;
                break;

            case "MainMenu":
                currentScene = Scene.Menu;
                DialoguesManager.instance.StartDialogue("Start");
                break;

            case "Tutorial":
                currentScene = Scene.Tutorial;
                // Prima facciamo partire il prologo: se l'inizializzazione della pesca
                // lancia un'eccezione, il prologo (e il FadeIn) partono comunque.
                StartCoroutine(StartPrologueDialogue());
                try
                {
                    FishingManager.instance.Initialize();
                }
                catch (Exception e)
                {
                    Debug.LogError("[GameManager] Errore in FishingManager.Initialize nella scena Tutorial:");
                    Debug.LogException(e);
                }
                break;

            default:
                currentScene = Scene.Ending;
                //Fai partire il dialogo finale
                break;
        }
        UpdateMoodAndAffection();
        Debug.Log("Setup scena " + currentScene);
    }

    public void ChangeScene(string sceneName)
    {
        StartCoroutine(UIManager.instance.FadeOut(1f, sceneName));
    }

    public void SkipIntro()
    {
        tutorialState = 3;
        boatLevel = 1;
        day = 2;
        SetYarnVariable("$dayIndex", 2);
        UpdateGrandsonHere();
        currentScene = Scene.Village;
        ChangeScene("MorningVillage");
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
        SFXManager.Play(SFX.CostruzioneBarca);
        builtBoat = true;
        AddMoney(-boatCosts[boatLevel]);
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
        float nextVisitDay = -1;
        yarnVariableStorage.TryGetValue("$visitIndex", out nextVisitDay);
        Debug.Log(day + " = " + nextVisitDay);
        if (nextVisitDay == day)
            grandsonHere = true;
        else
            grandsonHere = false;
    }

    private void UpdateMoodAndAffection()
    {
        if (currentScene != Scene.Menu && currentScene != Scene.Tutorial)
        {
            if (grandsonHere)
            {
                yarnVariableStorage.TryGetValue("$Affection", out affection);
                if (affection < -5)
                {
                    UIManager.instance.SetNeutralMood();
                }
                else
                {
                    UIManager.instance.SetHappyMood();
                }
                //cambia musica
            }
            else
            {
                UIManager.instance.SetSadMood();
                //cambia musica
            }
        }
    }

    public void GrandsonFollowing()
    {
        UIManager.instance.MakeGrandsonFollow();
    }

    public void GrandsonReturning()
    {
        UIManager.instance.MakeGrandsonGoAway();
    }

    #endregion Scene Transition

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
        // Realtime: se per qualsiasi motivo Time.timeScale fosse 0, WaitForSeconds non finirebbe mai.
        yield return new WaitForSecondsRealtime(1f);

        // Aspetta (max 3 s) che non ci siano altri dialoghi in corso, es. "Start" del menu.
        float waited = 0f;
        while (DialoguesManager.instance != null && DialoguesManager.instance.IsBusy && waited < 3f)
        {
            waited += Time.unscaledDeltaTime;
            yield return null;
        }

        if (DialoguesManager.instance == null)
        {
            Debug.LogError("[GameManager] DialoguesManager.instance è null: impossibile avviare il Prologo.");
        }
        else
        {
            DialoguesManager.instance.StartDialogue("Prologo");
            // L'avvio può essere rimandato di un attimo: aspettiamo fino a 3 s che parta.
            float waitedStart = 0f;
            while (!DialoguesManager.instance.isTalking && waitedStart < 3f)
            {
                waitedStart += Time.unscaledDeltaTime;
                yield return null;
            }
            if (DialoguesManager.instance.isTalking)
                yield break;
            Debug.LogError("[GameManager] Il dialogo 'Prologo' non è partito (vedi i messaggi sopra nel log).");
        }

        // Rete di sicurezza: se il Prologo non parte, almeno togliamo il nero
        // così il gioco non resta bloccato.
        Time.timeScale = 1;
        FadeIn();
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
