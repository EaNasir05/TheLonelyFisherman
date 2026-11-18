using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

[Serializable]
public class SceneMusic
{
    [Tooltip("Nome esatto della scena, come in Build Settings (es. MorningVillage).")]
    public string sceneName;
    [Tooltip("Traccia quando il nonno è da solo. Se vuota: silenzio.")]
    public AudioClip alone;
    [Tooltip("Traccia quando c'è anche il nipote. Se vuota usa quella 'Alone'.")]
    public AudioClip withGrandson;
}

/// <summary>
/// Musica di sottofondo. Ad ogni cambio scena sceglie la traccia in base alla scena
/// e alla presenza del nipote (GameManager.IsGrandsonHere), con dissolvenza incrociata.
/// Se la traccia giusta sta già suonando non la fa ripartire.
/// </summary>
public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [SerializeField]
    private List<SceneMusic> sceneMusic = new()
    {
        new SceneMusic { sceneName = "MainMenu" },
        new SceneMusic { sceneName = "MorningVillage" },
        new SceneMusic { sceneName = "EveningVillage" },
        new SceneMusic { sceneName = "Tutorial" },
        new SceneMusic { sceneName = "Boat" },
        new SceneMusic { sceneName = "Fishing" },
    };

    [Tooltip("Scene NON presenti nella lista: spuntato = continua la musica attuale, non spuntato = silenzio.")]
    [SerializeField] private bool keepMusicInUnlistedScenes = false;
    [SerializeField, Range(0f, 1f)] private float volume = 0.6f;
    [Tooltip("Durata della dissolvenza tra una traccia e l'altra, in secondi.")]
    [SerializeField] private float fadeDuration = 1.5f;
    [Tooltip("Opzionale: gruppo dell'AudioMixer per la musica.")]
    [SerializeField] private AudioMixerGroup outputGroup;

    private readonly AudioSource[] sources = new AudioSource[2];
    private int activeSource = 0;
    private AudioClip currentClip;
    private Coroutine fadeRoutine;

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

        for (int i = 0; i < sources.Length; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.loop = true;
            source.playOnAwake = false;
            source.volume = 0f;
            source.outputAudioMixerGroup = outputGroup;
            sources[i] = source;
        }
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
        if (instance == this)
            Refresh();
    }

    /// <summary>
    /// Ricalcola la traccia giusta per la scena attuale.
    /// Chiamalo anche se il nipote arriva o se ne va durante una scena.
    /// </summary>
    public void Refresh()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        SceneMusic rule = sceneMusic.Find(r => r.sceneName == sceneName);

        if (rule == null)
        {
            if (!keepMusicInUnlistedScenes)
                Play(null);
            return;
        }

        bool grandsonHere = GameManager.instance != null && GameManager.instance.IsGrandsonHere();
        AudioClip clip = grandsonHere && rule.withGrandson != null ? rule.withGrandson : rule.alone;
        Play(clip);
    }

    /// <summary>Passa a questa traccia con dissolvenza. null = sfuma nel silenzio.</summary>
    public void Play(AudioClip clip)
    {
        if (clip == currentClip)
            return;
        currentClip = clip;

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        AudioSource target = null;
        if (clip != null)
        {
            activeSource = 1 - activeSource;
            target = sources[activeSource];
            target.clip = clip;
            target.volume = 0f;
            target.Play();
        }
        fadeRoutine = StartCoroutine(Crossfade(target));
    }

    public void SetVolume(float value)
    {
        volume = Mathf.Clamp01(value);
        if (fadeRoutine == null && currentClip != null)
            sources[activeSource].volume = volume;
    }

    private IEnumerator Crossfade(AudioSource target)
    {
        float[] startVolumes = { sources[0].volume, sources[1].volume };
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            // Tempo reale: la musica sfuma anche quando il gioco è in pausa (Time.timeScale = 0).
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            for (int i = 0; i < sources.Length; i++)
            {
                float end = sources[i] == target ? volume : 0f;
                sources[i].volume = Mathf.Lerp(startVolumes[i], end, t);
            }
            yield return null;
        }

        for (int i = 0; i < sources.Length; i++)
        {
            if (sources[i] == target)
            {
                sources[i].volume = volume;
            }
            else
            {
                sources[i].volume = 0f;
                sources[i].Stop();
            }
        }
        fadeRoutine = null;
    }
}
