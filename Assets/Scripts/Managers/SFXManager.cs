using System.Collections;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;
    [SerializeField] private AudioSource[] sources;
    [SerializeField] private Vector2 randomPitchRange;
    private bool[] availableSources;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        availableSources = new bool[sources.Length];
        for (int i = 0; i < availableSources.Length; i++)
        {
            availableSources[i] = true;
        }
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

    public void PlayClip(AudioClip clip, float volume)
    {
        int i = GetTheFirstAvailableSource();
        if (i == -1)
            return;
        availableSources[i] = false;
        sources[i].clip = clip;
        sources[i].volume = volume;
        sources[i].loop = false;
        sources[i].Play();
        StartCoroutine(MakeSourceAvailable(clip.length, i));
    }

    public int PlayClip(AudioClip clip, float volume, float pitch)
    {
        int i = GetTheFirstAvailableSource();
        if (i == -1)
            return -1;
        availableSources[i] = false;
        sources[i].clip = clip;
        sources[i].volume = volume;
        sources[i].loop = false;
        sources[i].pitch = pitch;
        sources[i].Play();
        StartCoroutine(MakeSourceAvailable(clip.length, i));
        return i;
    }

    public void PlayClipWithRandomPitch(AudioClip clip, float volume)
    {
        int i = GetTheFirstAvailableSource();
        if (i == -1)
            return;
        availableSources[i] = false;
        sources[i].clip = clip;
        sources[i].volume = volume;
        sources[i].loop = false;
        sources[i].pitch = Random.Range(randomPitchRange.x, randomPitchRange.y);
        sources[i].Play();
        StartCoroutine(MakeSourceAvailable(clip.length, i));
    }

    public int PlayClipWithRandomPitchAndReturnIndex(AudioClip clip, float volume)
    {
        int i = GetTheFirstAvailableSource();
        if (i == -1)
            return -1;
        availableSources[i] = false;
        sources[i].clip = clip;
        sources[i].volume = volume;
        sources[i].loop = false;
        sources[i].pitch = Random.Range(randomPitchRange.x, randomPitchRange.y);
        sources[i].Play();
        StartCoroutine(MakeSourceAvailable(clip.length, i));
        return i;
    }

    public int PlayClipInLoop(AudioClip clip, float volume)
    {
        int i = GetTheFirstAvailableSource();
        if (i == -1)
            return -1;
        availableSources[i] = false;
        sources[i].clip = clip;
        sources[i].volume = volume;
        sources[i].loop = true;
        sources[i].pitch = Random.Range(0.9f, 1.1f);
        sources[i].Play();
        return i;
    }

    private IEnumerator MakeSourceAvailable(float delay, int index)
    {
        yield return new WaitForSeconds(delay);
        availableSources[index] = true;
    }

    public void StopClip(int index)
    {
        sources[index].Stop();
        availableSources[index] = true;
    }
}
