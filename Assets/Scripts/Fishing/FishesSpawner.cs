using System;
using System.Collections.Generic;
using System.IO.Compression;
using Unity.Collections;
using UnityEngine;

public class FishesSpawner : MonoBehaviour
{
    public static FishesSpawner instance;
    [SerializeField] private FishesList fishesList;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private int layersPerBiome = 4; //Quanti layer compongono un bioma
    [SerializeField] private List<int> fishesOnLayerPerBiome; //Per ogni bioma, quanti pesci sono presenti in un layer
    [SerializeField] private int numberOfLayersToSpawnOnStart = 3; //Quanti layer di pesci attivare con Initialize
    [SerializeField] private float unitsVisibleToCamera = 11; //Unità comprese nel campo visivo verticale della camera
    [SerializeField] private Vector2 fishesPositionXRange;
    private float layersSize = 5f; //Altezza di un layer
    private List<Dictionary<string, int>> fishesInLayers; //Quantità di pesci specifici per ogni layer
    private Dictionary<string, int> totalFishes; //Quantità totale di ogni tipo di pesce nel mare
    private int currentLayer = 0; //Layer in cui si trova la camera
    private int layersVisible = 3; //Quanti layer possono essere compresi nel campo visivo della camera sopra e sotto il currentLayer
    private List<Dictionary<string, List<GameObject>>> fishesActivatedPerLayer; //Pesci attivi per ogni layer

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (cameraTransform.position.y > currentLayer * -layersSize)
            DecreaseCurrentLayer();
        else if (cameraTransform.position.y < (currentLayer + 1) * -layersSize)
            IncreaseCurrentLayer();
    }

    public void Initialize()
    {
        int biomes = FishingManager.instance.biomesUnlocked;
        int layers = biomes * layersPerBiome + 1;
        layersSize = FishingManager.instance.biomesHeight / layersPerBiome;
        layersVisible = (int) Math.Ceiling((unitsVisibleToCamera - layersSize) / layersSize);
        fishesInLayers = new(new Dictionary<string, int>[layers]);
        totalFishes = new();
        fishesActivatedPerLayer = new(new Dictionary<string, List<GameObject>>[layers]);
        List<FishPercentages> fishesPercentagePerBiomes = fishesList.fishesRatesPerBiome;
        int y = -1;
        Dictionary<string, int> fishesInLayer = null;
        for (int x = 0; x < layers; x++)
        {
            if ((x % layersPerBiome == 0 && y + 1 < biomes) || y == -1)
            {
                y++;
                fishesInLayer = new();
                for (int z = 0; z < fishesPercentagePerBiomes[y].fishes.Count; z++)
                {
                    fishesInLayer.Add(fishesPercentagePerBiomes[y].fishes[z].fishType, fishesOnLayerPerBiome[y] * fishesPercentagePerBiomes[y].fishes[z].percentage / 100);
                }
            }
            fishesInLayers[x] = fishesInLayer;
            foreach (var kvp in fishesInLayer)
            {
                if (totalFishes.ContainsKey(kvp.Key))
                    totalFishes[kvp.Key] += kvp.Value;
                else
                    totalFishes.Add(kvp.Key, kvp.Value);
            }
        }
        FishesPool.instance.Initialize(totalFishes);
        for (int i = 0; i < numberOfLayersToSpawnOnStart; i++)
        {
            ActivateFishesOnLayer(i);
        }
    }

    private void IncreaseCurrentLayer()
    {
        if (currentLayer - layersVisible >= 0)
            DeactivateFishesOnLayer(currentLayer - layersVisible);
        currentLayer++;
        if (currentLayer + layersVisible < fishesInLayers.Count)
            ActivateFishesOnLayer(currentLayer + layersVisible);
    }

    private void DecreaseCurrentLayer()
    {
        if (currentLayer + layersVisible < fishesInLayers.Count)
            DeactivateFishesOnLayer(currentLayer + layersVisible);
        currentLayer--;
        if (currentLayer - layersVisible >= 0)
            ActivateFishesOnLayer(currentLayer - layersVisible);
    }

    private Vector2 GetRandomPositionInLayer(int layer)
    {
        return new Vector2(UnityEngine.Random.Range(fishesPositionXRange.x, fishesPositionXRange.y), UnityEngine.Random.Range(layer * -layersSize, (layer + 1) * -layersSize));
    }

    private void ActivateFishesOnLayer(int layer)
    {
        foreach (var kvp in fishesInLayers[layer])
        {
            List<GameObject> fishes = new();
            for (int j = 0; j < kvp.Value; j++)
            {
                fishes.Add(FishesPool.instance.ActivateFish(GetRandomPositionInLayer(layer), kvp.Key));
            }
            if (fishesActivatedPerLayer[layer] == null)
                fishesActivatedPerLayer[layer] = new();
            fishesActivatedPerLayer[layer][kvp.Key] = fishes;
        }
    }

    private void DeactivateFishesOnLayer(int layer)
    {
        foreach (var kvp in fishesActivatedPerLayer[layer])
        {
            for (int j = 0; j < kvp.Value.Count; j++)
            {
                FishesPool.instance.DeactivateFish(kvp.Value[j], kvp.Key);
            }
        }
    }
}
