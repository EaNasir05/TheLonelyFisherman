using System;
using UnityEngine;

public class FishingManager : MonoBehaviour
{
    public static FishingManager instance;
    public bool stop = false;
    public int points = 0;
    private int biomesUnlocked = 1;
    public float biomesHeight = 20f;
    [SerializeField] private float pointsFinalMultiplier = 1;
    [SerializeField] private int maxAttemptsWithoutGrandson = 1;
    [SerializeField] private int maxAttemptsWithGrandson = 2;
    [SerializeField] private UpgradesList upgradesList;
    private Hook hook;
    private int maxAttempts = 3;
    private int attemptsRemaining = 3;

    public int GetBiomesUnlocked() => biomesUnlocked;

    private void Awake()
    {
        instance = this;
    }

    #region Initialize

    public void Initialize()
    {
        hook = FindFirstObjectByType<Hook>();
        CheckUpgrades();
        if (GameManager.instance.IsGrandsonHere())
            maxAttempts = maxAttemptsWithGrandson;
        else
            maxAttempts = maxAttemptsWithoutGrandson;
        attemptsRemaining = maxAttempts;
        UIManager.instance.UpdateAttemptsSlider(maxAttempts, attemptsRemaining);
        FishesSpawner.instance.Initialize();
    }

    private void CheckUpgrades()
    {
        if (upgradesList.GetUpgradeByName("Lunghezza lenza 3").unlocked)
        {
            biomesUnlocked = 4;
            hook.weightGainMultiplier = 0.7f;
        }
        else if (upgradesList.GetUpgradeByName("Lunghezza lenza 2").unlocked)
        {
            biomesUnlocked = 3;
            hook.weightGainMultiplier = 0.8f;
        }
        else if (upgradesList.GetUpgradeByName("Lunghezza lenza 1").unlocked)
        {
            biomesUnlocked = 2;
            hook.weightGainMultiplier = 0.9f;
        }
        else
        {
            biomesUnlocked = 1;
            hook.weightGainMultiplier = 1f;
        }

        if (upgradesList.GetUpgradeByName("Manovrabilità galleggiante 3").unlocked)
        {
            hook.horizontalSpeedMultiplier = 1.75f;
        }
        else if (upgradesList.GetUpgradeByName("Manovrabilità galleggiante 2").unlocked)
        {
            hook.horizontalSpeedMultiplier = 1.5f;
        }
        else if (upgradesList.GetUpgradeByName("Manovrabilità galleggiante 1").unlocked)
        {
            hook.horizontalSpeedMultiplier = 1.25f;
        }
        else
        {
            hook.horizontalSpeedMultiplier = 1f;
        }

        if (upgradesList.GetUpgradeByName("Potenza amo 3").unlocked)
        {
            hook.power = 22;
        }
        else if (upgradesList.GetUpgradeByName("Potenza amo 2").unlocked)
        {
            hook.power = 15;
        }
        else if (upgradesList.GetUpgradeByName("Potenza amo 1").unlocked)
        {
            hook.power = 7;
        }
        else
        {
            hook.power = 0;
        }

        if (upgradesList.GetUpgradeByName("Abbattimento pesci 2").unlocked)
        {
            hook.criticalCatchPointsMultiplier = 1.5f;
        }
        else if (upgradesList.GetUpgradeByName("Abbattimento pesci 1").unlocked)
        {
            hook.criticalCatchPointsMultiplier = 1.25f;
        }
        else
        {
            hook.criticalCatchPointsMultiplier = 1f;
        }

        if (upgradesList.GetUpgradeByName("Piombino equilibrato 3").unlocked)
        {
            hook.fishesToLoseOnOverweight = 3;
        }
        else if (upgradesList.GetUpgradeByName("Piombino equilibrato 2").unlocked)
        {
            hook.fishesToLoseOnOverweight = 4;
        }
        else if (upgradesList.GetUpgradeByName("Piombino equilibrato 1").unlocked)
        {
            hook.fishesToLoseOnOverweight = 5;
        }
        else
        {
            hook.fishesToLoseOnOverweight = 6;
        }

        if (upgradesList.GetUpgradeByName("Manico ergonomico 3").unlocked)
        {
            hook.staminaLossMultiplier = 0.55f;
        }
        else if (upgradesList.GetUpgradeByName("Manico ergonomico 2").unlocked)
        {
            hook.staminaLossMultiplier = 0.7f;
        }
        else if (upgradesList.GetUpgradeByName("Manico ergonomico 1").unlocked)
        {
            hook.staminaLossMultiplier = 0.85f;
        }
        else
        {
            hook.staminaLossMultiplier = 1f;
        }

        if (upgradesList.GetUpgradeByName("Mulinello magico 3").unlocked)
        {
            hook.staminaConsumedMultiplierOnSpeedUp = 0.7f;
            hook.staminaConsumedMultiplierOnSpeedDown = 0.7f;
        }
        else if (upgradesList.GetUpgradeByName("Mulinello magico 2").unlocked)
        {
            hook.staminaConsumedMultiplierOnSpeedUp = 0.8f;
            hook.staminaConsumedMultiplierOnSpeedDown = 0.8f;
        }
        else if (upgradesList.GetUpgradeByName("Mulinello magico 1").unlocked)
        {
            hook.staminaConsumedMultiplierOnSpeedUp = 0.9f;
            hook.staminaConsumedMultiplierOnSpeedDown = 0.9f;
        }
        else
        {
            hook.staminaConsumedMultiplierOnSpeedUp = 1f;
            hook.staminaConsumedMultiplierOnSpeedDown = 1f;
        }

        if (upgradesList.GetUpgradeByName("Amo incantato 3").unlocked)
        {
            hook.criticalChanceWithMagic = 20;
        }
        else if (upgradesList.GetUpgradeByName("Amo incantato 2").unlocked)
        {
            hook.criticalChanceWithMagic = 15;
        }
        else if (upgradesList.GetUpgradeByName("Amo incantato 1").unlocked)
        {
            hook.criticalChanceWithMagic = 10;
        }
        else
        {
            hook.criticalChanceWithMagic = 5;
        }

        if (upgradesList.GetUpgradeByName("Focus arcano 2").unlocked)
        {
            hook.criticalChanceMultiplierWithMagicOnReturn = 3;
        }
        else if (upgradesList.GetUpgradeByName("Focus arcano 1").unlocked)
        {
            hook.criticalChanceMultiplierWithMagicOnReturn = 2;
        }
        else
        {
            hook.criticalChanceMultiplierWithMagicOnReturn = 1;
        }
    }

    #endregion Initialize

    #region Points

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

    #endregion Points

    public void StartFishingAttempt()
    {
        if (attemptsRemaining > 0)
        {
            attemptsRemaining--;
            UIManager.instance.UpdateAttemptsSlider(maxAttempts, attemptsRemaining);
        }
    }

    public void CheckFishingAttempts()
    {
        if (attemptsRemaining == 0)
        {
            if (!GameManager.instance.tutorial)
            {
                stop = true;
                int money = (int)(points * pointsFinalMultiplier);
                GameManager.instance.AddMoney(money);
                UIManager.instance.ShowFishingResults(money);
            }
            else if (GameManager.instance.GetDay() == 1)
            {
                //Inizia dialogo
            }
        }
    }
}
