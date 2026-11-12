using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FishesList", menuName = "Scriptable Objects/FishesList")]
public class FishesList : ScriptableObject
{
    public List<Fish> list;
    public List<FishPercentages> fishesRatesPerBiome;

    public GameObject GetFishPrefabByType(string type)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].type == type)
                return list[i].prefab;
        }
        return null;
    }
}

[Serializable]
public class Fish
{
    public string type;
    public GameObject prefab;
}

[Serializable]
public class FishPercentages
{
    public List<FishPercentagePerBiome> fishes;
}

[Serializable]
public class FishPercentagePerBiome
{
    public string fishType;
    [Range(1, 100)] public int percentage;
}
