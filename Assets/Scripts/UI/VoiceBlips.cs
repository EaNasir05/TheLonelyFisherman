using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using Yarn.Markup;
using Yarn.Unity;
using Random = UnityEngine.Random;

[Serializable]
public class CharacterVoice
{
    [Tooltip("Nome del personaggio come scritto nello Yarn prima dei due punti (es. Nonno).")]
    public string characterName;
    [Tooltip("Suoni della voce: ad ogni blip ne viene scelto uno a caso. Se vuoto, usa un beep generato.")]
    public AudioClip[] blips;
    [Range(0f, 1f)] public float volume = 0.3f;
    [Tooltip("Tono della voce: < 1 più grave, > 1 più acuta.")]
    [Range(0.3f, 3f)] public float pitch = 1f;
    [Tooltip("Variazione casuale del tono ad ogni blip: rende la voce meno robotica.")]
    [Range(0f, 0.5f)] public float pitchVariation = 0.05f;
    [Tooltip("Un blip ogni quante lettere per questo personaggio (più alto = parla più lento/pacato). 0 = usa il valore generale.")]
    [Min(0)] public int everyNLetters = 0;
}

/// <summary>
/// Effetto "voce" stile Undertale / Pokémon Mystery Dungeon: un piccolo suono mentre
/// le lettere compaiono. Va aggiunto alla lista "Event Handlers" del LinePresenter di Yarn.
/// Richiede che sul LinePresenter sia attivo "Use Typewriter Effect".
/// </summary>
public class VoiceBlips : ActionMarkupHandler
{
    [SerializeField]
    private List<CharacterVoice> voices = new()
    {
        new CharacterVoice { characterName = "Nonno", pitch = 0.8f },
        new CharacterVoice { characterName = "Albert", pitch = 1.3f },
        new CharacterVoice { characterName = "Riccardo", pitch = 1.05f },
    };

    [Header("Righe senza nome (narratore, #testo)")]
    [Tooltip("Spunta se vuoi i blip anche per le righe del narratore.")]
    [SerializeField] private bool narratorHasVoice = false;
    [SerializeField] private CharacterVoice narratorVoice = new CharacterVoice { characterName = "Narratore" };

    [Header("Generale")]
    [Tooltip("Un blip ogni quante lettere: 1 = ogni lettera, 2 = una sì e una no (più naturale)...")]
    [SerializeField, Min(1)] private int everyNLetters = 2;
    [Tooltip("Voce per i personaggi che non sono nella lista.")]
    [SerializeField] private CharacterVoice defaultVoice = new CharacterVoice { characterName = "Default" };
    [Tooltip("Opzionale: gruppo dell'AudioMixer.")]
    [SerializeField] private AudioMixerGroup outputGroup;

    private AudioSource source;
    private AudioClip generatedBlip;
    private CharacterVoice currentVoice;
    private TMP_Text currentText;
    private string currentPlainText = "";
    private int lettersSinceLastBlip;

    private void Awake()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = false;
        source.outputAudioMixerGroup = outputGroup;
        generatedBlip = CreateBeep();
    }

    #region Yarn callbacks

    public override void OnPrepareForLine(MarkupParseResult line, TMP_Text text) { }

    public override void OnLineDisplayBegin(MarkupParseResult line, TMP_Text text)
    {
        currentText = text;
        currentPlainText = line.Text ?? "";
        lettersSinceLastBlip = 1000; // così la prima lettera suona subito

        string characterName = GetCharacterName(line);
        if (string.IsNullOrEmpty(characterName))
            currentVoice = narratorHasVoice ? narratorVoice : null;
        else
            currentVoice = FindVoice(characterName) ?? defaultVoice;
    }

    public override YarnTask OnCharacterWillAppear(int currentCharacterIndex, MarkupParseResult line, CancellationToken cancellationToken)
    {
        // Quando il giocatore salta l'animazione, le lettere rimanenti compaiono tutte
        // nello stesso istante: niente blip, altrimenti suonerebbero tutti insieme.
        if (cancellationToken.IsCancellationRequested || currentVoice == null)
            return YarnTask.CompletedTask;

        if (char.IsLetterOrDigit(GetCharacter(currentCharacterIndex)))
        {
            lettersSinceLastBlip++;
            int every = currentVoice.everyNLetters > 0 ? currentVoice.everyNLetters : everyNLetters;
            if (lettersSinceLastBlip >= every)
            {
                lettersSinceLastBlip = 0;
                PlayBlip(currentVoice);
            }
        }
        return YarnTask.CompletedTask;
    }

    public override void OnLineDisplayComplete() { }

    public override void OnLineWillDismiss() { }

    #endregion Yarn callbacks

    private static string GetCharacterName(MarkupParseResult line)
    {
        if (line.TryGetAttributeWithName("character", out var attribute)
            && attribute.Properties.TryGetValue("name", out var value))
        {
            return value.StringValue;
        }
        return null;
    }

    private CharacterVoice FindVoice(string characterName)
    {
        string name = characterName.Trim();
        foreach (CharacterVoice voice in voices)
        {
            if (string.Equals(voice.characterName?.Trim(), name, StringComparison.OrdinalIgnoreCase))
                return voice;
        }
        return null;
    }

    private char GetCharacter(int index)
    {
        // Indice dei caratteri visibili di TextMeshPro (esclude i tag di formattazione).
        if (currentText != null && currentText.textInfo != null && index < currentText.textInfo.characterCount)
            return currentText.textInfo.characterInfo[index].character;
        if (index < currentPlainText.Length)
            return currentPlainText[index];
        return ' ';
    }

    private void PlayBlip(CharacterVoice voice)
    {
        AudioClip clip = voice.blips != null && voice.blips.Length > 0
            ? voice.blips[Random.Range(0, voice.blips.Length)]
            : generatedBlip;
        if (clip == null)
            return;

        // Interrompe il blip precedente: dà il classico effetto "a scatti".
        source.Stop();
        source.clip = clip;
        source.volume = voice.volume;
        source.pitch = voice.pitch + Random.Range(-voice.pitchVariation, voice.pitchVariation);
        source.Play();
    }

    /// <summary>Genera un breve beep a onda quadra, simile a quello di Undertale.</summary>
    private static AudioClip CreateBeep()
    {
        const int sampleRate = 44100;
        const float frequency = 520f;
        const float duration = 0.045f;
        int samples = Mathf.CeilToInt(sampleRate * duration);
        float[] data = new float[samples];
        int fade = Mathf.Min(220, samples / 4); // piccola dissolvenza per evitare "click"
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            float square = Mathf.Sin(2f * Mathf.PI * frequency * t) >= 0f ? 1f : -1f;
            float envelope = Mathf.Min(1f, Mathf.Min(i, samples - 1 - i) / (float)fade);
            data[i] = square * 0.12f * envelope;
        }
        AudioClip clip = AudioClip.Create("GeneratedBlip", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }
}
