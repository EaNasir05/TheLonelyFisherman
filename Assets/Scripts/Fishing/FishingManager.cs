using System;
using System.Drawing;
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

    public void GainPoints(int value)
    {
        points += value;
        UIManager.instance.UpdatePointsText(points);
        UIManager.instance.PrintPointGainText(value);
    }

    public void LosePoints(int value)
    {
        points = Math.Clamp(points - value, 0, 999999999);
        UIManager.instance.UpdatePointsText(points);
        UIManager.instance.PrintPointLossText(value);
    }

    public void StartFishingAttempt()
    {
        if (attemptsRemaining > 0)
        {
            attemptsRemaining--;
        }
    }
}
