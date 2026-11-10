using UnityEngine;

public class FishingManager : MonoBehaviour
{
    public static FishingManager instance;
    public bool stop = false;
    public int points = 0;
    public int biomesUnlocked = 1;
    public float biomesHeight = 20f;
    private int attemptsRemaining = 3;

    private void Awake()
    {
        instance = this;
    }

    public void Initialize()
    {
        FishesSpawner.instance.Initialize();
    }

    public void StartFishingAttempt()
    {
        if (attemptsRemaining > 0)
        {
            attemptsRemaining--;
        }
    }
}
