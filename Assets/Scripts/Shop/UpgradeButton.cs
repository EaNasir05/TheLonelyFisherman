using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{
    private EventSystem eventSystem;
    [SerializeField] private string upgradeName;
    [SerializeField] private UpgradesList upgradesList;
    private Upgrade upgrade;
    [SerializeField] private bool firstNode;
    [SerializeField] private List<UpgradeButton> nextUpgrades;
    [SerializeField] private Button button;
    [SerializeField] private Image buttonImage;
    [SerializeField] private GameObject unlockedSymbol;
    [SerializeField] private Image connectionImage;
    public bool unlocked;
    public bool available;
    public Action onPurchase;

    private void Awake()
    {
        upgrade = upgradesList.GetUpgradeByName(upgradeName);
    }

    private void Start()
    {
        eventSystem = FindFirstObjectByType<EventSystem>();
        if (firstNode)
        {
            UpdateButton(true);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (!DialoguesManager.instance.isTalking)
            UpdateUpgradeInfo();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (available && !DialoguesManager.instance.isTalking)
            eventSystem.SetSelectedGameObject(gameObject);
    }

    private void UpdateButton(bool available)
    {
        this.available = available;
        button.interactable = available;
        unlockedSymbol.SetActive(false);
        if (available)
        {
            if (connectionImage != null)
                connectionImage.color = Color.white;
            if (upgrade.unlocked)
            {
                unlocked = true;
                unlockedSymbol.SetActive(true);
                buttonImage.sprite = upgrade.icon;
                foreach (UpgradeButton button in nextUpgrades)
                {
                    button.UpdateButton(true);
                }
            }
            else
            {
                unlocked = false;
                buttonImage.sprite = upgrade.icon;
                foreach (UpgradeButton button in nextUpgrades)
                {
                    button.UpdateButton(false);
                }
            }
        }
        else
        {
            buttonImage.sprite = upgrade.lockedIcon;
            if (connectionImage != null)
                connectionImage.color = Color.gray;
            foreach (UpgradeButton button in nextUpgrades)
            {
                button.UpdateButton(false);
            }
        }
    }

    public void UnlockUpgrade()
    {
        if (available && !unlocked && !DialoguesManager.instance.isTalking)
        {
            int cost = upgrade.cost;
            if (GameManager.instance.GetMoney() >= cost)
            {
                SFXManager.Play(SFX.Money);
                GameManager.instance.AddMoney(-cost);
                upgrade.unlocked = true;
                UpdateButton(true);
                UpdateUpgradeInfo();
                onPurchase?.Invoke();
            }
            else
            {
                SFXManager.Play(SFX.PurchaseFailed);
            }
        }
    }

    private void UpdateUpgradeInfo()
    {
        UIManager.instance.UpdateUpgradeInfo(upgrade.name, upgrade.description, upgrade.effect, upgrade.cost, upgrade.icon, upgrade.unlocked);
    }
}
