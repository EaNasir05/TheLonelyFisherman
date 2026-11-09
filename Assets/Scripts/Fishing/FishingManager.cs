using UnityEngine;

public class FishingManager : MonoBehaviour
{
    public static FishingManager instance;
    public bool stop = false;
    public int points = 0;
    public float stamina = 100f;
    private int attemptsRemaining = 3;

    private void Awake()
    {
        instance = this;
    }

    public void StartFishingAttempt()
    {
        if (attemptsRemaining > 0)
        {
            attemptsRemaining--;
        }
    }
}
