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
    [SerializeField] private UpgradesList upgradesList;
    private int attemptsRemaining = 3;

    private void Awake()
    {
        instance = this;
    }

    public void Initialize()
    {
        CheckUpgrades();
        FishesSpawner.instance.Initialize();
    }

    private void CheckUpgrades()
    {
        //controlla se gli upgrade son sbloccati
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
