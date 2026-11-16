using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradesList", menuName = "Scriptable Objects/UpgradesList")]
public class UpgradesList : ScriptableObject
{
    public List<Upgrade> list;

    public Upgrade GetUpgradeByName(string name)
    {
        foreach (Upgrade upgrade in list)
        {
            if (upgrade.name == name)
                return upgrade;
        }
        return null;
    }

    public void ResetUpgrades()
    {
        foreach (Upgrade upgrade in list)
        {
            upgrade.unlocked = false;
        }
    }
}

[Serializable]
public class Upgrade
{
    public string name;
    public string description;
    public string effect;
    public Sprite icon;
    public Sprite lockedIcon;
    public bool unlocked;
    public int cost;
}
