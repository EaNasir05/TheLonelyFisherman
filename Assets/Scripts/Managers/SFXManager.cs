using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum SFX
{
    AmoRising,
    ButtonClick,
    CamminataLoop,
    CostruzioneBarca,
    CriticalHit,
    NormalHit,
    HookLanding,
    HookThrow,
    MagicHit,
    Money,
    SlowedMagic,
    Sovraccarico,
    ShopOpen,
    ExitTheSea,
    PurchaseFailed
}

[Serializable]
public class SFXEntry
{
    public SFX sound;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    [Tooltip("Variazione casuale del tono, utile per suoni ripetuti (colpi, passi).")]
    public bool randomPitch = false;
}

/// <summary>
/// Effetti sonori. Uso dagli altri script:
///   SFXManager.Play(SFX.HookThrow);                 // suono singolo
///   int id = SFXManager.PlayLoop(SFX.CamminataLoop); // suono in loop
///   SFXManager.StopLoop(id);                        // ferma il loop
/// I clip si assegnano nell'Inspector. Sopravvive ai cambi scena.
/// </summary>
public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    [SerializeField]
    private SFXEntry[] sounds =
    {
        new SFXEntry { sound = SFX.AmoRising },
        new SFXEntry { sound = SFX.ButtonClick },
        new SFXEntry { sound = SFX.CamminataLoop },
        new SFXEntry { sound = SFX.CostruzioneBarca },
        new SFXEntry { sound = SFX.CriticalHit, randomPitch = true },
        new SFXEntry { sound = SFX.NormalHit, randomPitch = true },
        new SFXEntry { sound = SFX.HookLanding },
        new SFXEntry { sound = SFX.HookThrow },
        new SFXEntry { sound = SFX.MagicHit, randomPitch = true },
        new SFXEntry { sound = SFX.Money },
        new SFXEntry { sound = SFX.SlowedMagic },
        new SFXEntry { sound = SFX.Sovraccarico },
        new SFXEntry { sound = SFX.ShopOpen },
    };

    [Tooltip("Quanti suoni possono suonare contemporaneamente.")]
    [SerializeField] private int voices = 10;
    [SerializeField] private Vector2 randomPitchRange = new Vector2(0.9f, 1.1f);
    [Tooltip("Opzionale: gruppo dell'AudioMixer per gli effetti.")]
    [SerializeField] private AudioMixerGroup outputGroup;
    [Tooltip("Aggiunge in automatico il suono ButtonClick a tutti i bottoni UI di ogni scena.")]
    [SerializeField] private bool autoButtonClick = true;

    private AudioSource[] sources;
    private bool[] availableSources;
    private int[] loopIds;
    private int nextLoopId = 1;
    private UnityAction buttonClickAction;

    #region Setup

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            enabled = false;
            Destroy(this);
            return;
        }
        instance = this;
        if (transform.parent == null)
            DontDestroyOnLoad(gameObject);

        voices = Mathf.Max(1, voices);
        sources = new AudioSource[voices];
        availableSources = new bool[voices];
        loopIds = new int[voices];
        for (int i = 0; i < voices; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.outputAudioMixerGroup = outputGroup;
            sources[i] = source;
            availableSources[i] = true;
        }
        buttonClickAction = () => Play(SFX.ButtonClick);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        if (instance != this)
            return;

        // I loop (passi, amo che risale) appartengono alla scena precedente: fermali.
        // I suoni singoli invece continuano (es. CostruzioneBarca durante il cambio scena).
        StopAllLoops();

        if (autoButtonClick)
            HookButtons();
    }

    private void HookButtons()
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Button button in buttons)
        {
            // Remove + Add: così un bottone non riceve mai il suono due volte.
            button.onClick.RemoveListener(buttonClickAction);
            button.onClick.AddListener(buttonClickAction);
        }
    }

    #endregion Setup

    #region API statica

    /// <summary>Suona un effetto una volta.</summary>
    public static void Play(SFX sound)
    {
        if (instance != null)
            instance.PlayOneShot(sound);
    }

    /// <summary>Avvia un effetto in loop. Restituisce un id da passare a StopLoop (0 = non partito).</summary>
    public static int PlayLoop(SFX sound)
    {
        return instance != null ? instance.StartLoop(sound) : 0;
    }

    /// <summary>Ferma un loop avviato con PlayLoop.</summary>
    public static void StopLoop(int loopId)
    {
        if (instance != null && loopId != 0)
            instance.StopLoopInternal(loopId);
    }

    #endregion API statica

    #region Implementazione

    private SFXEntry GetEntry(SFX sound)
    {
        foreach (SFXEntry entry in sounds)
        {
            if (entry.sound == sound)
                return entry.clip != null ? entry : null;
        }
        return null;
    }

    private int GetTheFirstAvailableSource()
    {
        for (int i = 0; i < availableSources.Length; i++)
        {
            if (availableSources[i])
                return i;
        }
        return -1;
    }

    private void PlayOneShot(SFX sound)
    {
        SFXEntry entry = GetEntry(sound);
        if (entry == null)
            return;
        int i = GetTheFirstAvailableSource();
        if (i == -1)
            return;

        AudioSource source = sources[i];
        availableSources[i] = false;
        source.clip = entry.clip;
        source.volume = entry.volume;
        source.loop = false;
        source.pitch = entry.randomPitch ? Random.Range(randomPitchRange.x, randomPitchRange.y) : 1f;
        source.Play();
        StartCoroutine(MakeSourceAvailable(entry.clip.length / Mathf.Abs(source.pitch), i));
    }

    private int StartLoop(SFX sound)
    {
        SFXEntry entry = GetEntry(sound);
        if (entry == null)
            return 0;
        int i = GetTheFirstAvailableSource();
        if (i == -1)
            return 0;

        AudioSource source = sources[i];
        availableSources[i] = false;
        source.clip = entry.clip;
        source.volume = entry.volume;
        source.loop = true;
        source.pitch = entry.randomPitch ? Random.Range(randomPitchRange.x, randomPitchRange.y) : 1f;
        source.Play();
        loopIds[i] = nextLoopId++;
        return loopIds[i];
    }

    private void StopLoopInternal(int loopId)
    {
        for (int i = 0; i < loopIds.Length; i++)
        {
            if (loopIds[i] == loopId)
            {
                ReleaseSource(i);
                return;
            }
        }
    }

    private void StopAllLoops()
    {
        for (int i = 0; i < loopIds.Length; i++)
        {
            if (loopIds[i] != 0)
                ReleaseSource(i);
        }
    }

    private void ReleaseSource(int i)
    {
        sources[i].Stop();
        sources[i].loop = false;
        loopIds[i] = 0;
        availableSources[i] = true;
    }

    private IEnumerator MakeSourceAvailable(float delay, int index)
    {
        // Tempo reale: funziona anche quando il gioco è in pausa (Time.timeScale = 0).
        yield return new WaitForSecondsRealtime(delay);
        if (loopIds[index] == 0)
            availableSources[index] = true;
    }

    #endregion Implementazione
}
