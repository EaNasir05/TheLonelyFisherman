using System;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private Upgrade upgrade;
    [SerializeField] private bool firstNode;
    [SerializeField] private List<UpgradeButton> nextUpgrades;
    private Image buttonImage;
    public bool unlocked;
    public bool available;

    private void Start()
    {
        buttonImage = GetComponent<Image>();
        if (firstNode)
        {
            UpdateButton(true);
        }
    }

    public void UpdateButton(bool available)
    {
        this.available = available;
        if (available)
        {
            //aggiorna immagine con icona upgrade
            if (upgrade.unlocked)
            {
                unlocked = true;
                //colora di verde il bordo e aggiungi V verde
                foreach (UpgradeButton button in nextUpgrades)
                {
                    button.UpdateButton(true);
                }
            }
            else
            {
                unlocked = false;
                //colora di grigio il bordo
                foreach (UpgradeButton button in nextUpgrades)
                {
                    button.UpdateButton(false);
                }
            }
        }
        else
        {
            //aggiorna immagine con lucchetto
            foreach (UpgradeButton button in nextUpgrades)
            {
                button.UpdateButton(false);
            }
        }
    }

    public void UnlockUpgrade()
    {
        if (available && !unlocked)
        {
            int cost = upgrade.cost;
            if (GameManager.instance.GetMoney() >= cost)
            {
                //sfx acquisto
                GameManager.instance.AddMoney(-cost);
                //aggiorna conteggio monete
                upgrade.unlocked = true;
                UpdateButton(true);
            }
            else
            {
                //sfx acquisto rifiutato
            }
        }
    }

    public void UpdateUpgradeInfo()
    {
        //aggiorna titolo
        //aggiorna icona
        //aggiorna descrizione
        if (unlocked)
        {
            //aggiorna costo
            //nascondi unlocked
        }
        else
        {
            //aggiorna unlocked
            //nascondi costo
        }
        //aggiorna effetto
    }
}
