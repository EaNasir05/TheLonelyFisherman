using System;
using System.Collections;
using UnityEngine;
using Yarn.Unity;

public class DialoguesManager : MonoBehaviour
{
    public static DialoguesManager instance;
    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] private LinePresenter linePresenter;
    public bool isTalking = false;
    public Action<bool> onDialogueActivation;

    // Runner su cui abbiamo già registrato comandi e listener.
    // Se il runner cambia (es. quello vecchio è stato distrutto col cambio scena) li registriamo di nuovo.
    private DialogueRunner configuredRunner;
    private bool closeShopCommandAdded = false;
    private Coroutine pendingStart;

    /// <summary>True se c'è davvero un dialogo in corso.</summary>
    public bool IsBusy => isTalking && dialogueRunner != null && dialogueRunner.IsDialogueRunning;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            // Copia duplicata (es. il manager presente anche nella scena Tutorial):
            // la disattiviamo subito così non esegue nessuna logica prima di essere distrutta.
            enabled = false;
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (instance == this)
            EnsureReady();
    }

    /// <summary>
    /// Si assicura di avere un DialogueRunner valido e che comandi/listener siano registrati su di esso.
    /// Viene chiamato in modo "pigro" prima di ogni dialogo, così non dipende dall'ordine
    /// in cui Unity esegue Awake/OnEnable/sceneLoaded (che in build può essere diverso dall'Editor).
    /// </summary>
    private bool EnsureReady()
    {
        // "== null" è vero anche se il runner è stato distrutto insieme alla scena precedente.
        if (dialogueRunner == null)
        {
            dialogueRunner = FindFirstObjectByType<DialogueRunner>();
            if (dialogueRunner != null)
                Debug.LogWarning($"[DialoguesManager] Il riferimento al DialogueRunner era perso: ricollegato a '{dialogueRunner.name}'.");
        }
        if (linePresenter == null)
            linePresenter = FindFirstObjectByType<LinePresenter>();

        if (dialogueRunner == null)
        {
            Debug.LogError("[DialoguesManager] DialogueRunner non trovato.");
            return false;
        }

        if (configuredRunner != dialogueRunner)
        {
            // Wrapper invece di "GameManager.instance.Metodo": così il comando non dipende
            // da quale manager si è svegliato per primo.
            dialogueRunner.AddCommandHandler<string>("ChangeScene", CmdChangeScene);
            dialogueRunner.AddCommandHandler("FadeIn", CmdFadeIn);
            dialogueRunner.AddCommandHandler("NextDay", CmdNextDay);
            dialogueRunner.AddCommandHandler("GoFishing", CmdGoFishing);
            dialogueRunner.AddCommandHandler("BuildBoat", CmdBuildBoat);
            dialogueRunner.AddCommandHandler("GrandsonFollowing", CmdGrandsonFollowing);
            dialogueRunner.AddCommandHandler("GrandsonReturning", CmdGrandsonReturning);
            dialogueRunner.onDialogueComplete.AddListener(DialogueCompleted);
            configuredRunner = dialogueRunner;
            closeShopCommandAdded = false;
        }
        return true;
    }

    #region Command wrappers

    private void CmdChangeScene(string sceneName) => GameManager.instance.ChangeScene(sceneName);
    private void CmdFadeIn() => GameManager.instance.FadeIn();
    private void CmdNextDay() => GameManager.instance.NextDay();
    private void CmdGoFishing() => GameManager.instance.GoFishing();
    private void CmdBuildBoat() => GameManager.instance.BuildBoat();
    private void CmdGrandsonFollowing() => GameManager.instance.GrandsonFollowing();
    private void CmdGrandsonReturning() => GameManager.instance.GrandsonReturning();
    private void CmdCloseShopUnlocked() => UIManager.instance.ShowCloseShopButton();

    #endregion Command wrappers

    public void AddStupidFuckingCommand()
    {
        if (!EnsureReady() || closeShopCommandAdded)
            return;
        // Usa sempre lo UIManager della scena corrente (non quello di quando è stato registrato).
        dialogueRunner.AddCommandHandler("CloseShopUnlocked", CmdCloseShopUnlocked);
        closeShopCommandAdded = true;
    }

    public void StartDialogue(string nodeName)
    {
        if (!EnsureReady())
            return;

        // isTalking è ancora true ma Yarn ha già finito: o la fine del dialogo precedente
        // sta per arrivare (es. fade-out del LinePresenter), oppure è andata persa.
        // Aspettiamo un attimo e poi avviamo comunque il nuovo dialogo.
        if (isTalking && !dialogueRunner.IsDialogueRunning)
        {
            if (pendingStart != null)
                StopCoroutine(pendingStart);
            pendingStart = StartCoroutine(StartWhenFree(nodeName));
            return;
        }

        if (isTalking)
        {
            Debug.LogWarning($"[DialoguesManager] Dialogo '{nodeName}' ignorato: c'è già un dialogo in corso.");
            return;
        }

        BeginDialogue(nodeName);
    }

    private IEnumerator StartWhenFree(string nodeName)
    {
        float waited = 0f;
        while (isTalking && waited < 2f)
        {
            waited += Time.unscaledDeltaTime;
            yield return null;
        }
        pendingStart = null;

        if (isTalking)
        {
            if (dialogueRunner != null && dialogueRunner.IsDialogueRunning)
            {
                Debug.LogWarning($"[DialoguesManager] Dialogo '{nodeName}' ignorato: c'è già un dialogo in corso.");
                yield break;
            }
            Debug.LogWarning($"[DialoguesManager] isTalking era bloccato a true senza dialoghi attivi: lo resetto prima di '{nodeName}'.");
            isTalking = false;
        }

        if (EnsureReady())
            BeginDialogue(nodeName);
    }

    private void BeginDialogue(string nodeName)
    {
        isTalking = true;
        if (GameManager.instance.tutorialState == 1 || GameManager.instance.tutorialState == 4)
        {
            if (linePresenter != null)
                linePresenter.useFadeEffect = false;
            Time.timeScale = 0;
        }

        NotifyDialogueActivation(true);

        Debug.Log($"[DialoguesManager] Avvio dialogo '{nodeName}'.");
        dialogueRunner.StartDialogue(nodeName);
    }

    public void NextLine()
    {
        if (EnsureReady() && isTalking)
            dialogueRunner.RequestNextLine();
    }

    public void DialogueCompleted()
    {
        // NB: in Yarn Spinner 3.0 questo evento arriva quando IsDialogueRunning è ancora true,
        // quindi qui NON va controllato IsDialogueRunning.
        isTalking = false;
        NotifyDialogueActivation(false);
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            if (linePresenter != null)
                linePresenter.useFadeEffect = false;
        }
    }

    /// <summary>
    /// Avvisa gli iscritti uno per uno: se uno lancia un'eccezione (es. un oggetto della scena
    /// precedente già distrutto) gli altri vengono comunque chiamati e il dialogo parte lo stesso.
    /// </summary>
    private void NotifyDialogueActivation(bool value)
    {
        if (onDialogueActivation == null)
            return;
        foreach (Delegate handler in onDialogueActivation.GetInvocationList())
        {
            try
            {
                ((Action<bool>)handler).Invoke(value);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
