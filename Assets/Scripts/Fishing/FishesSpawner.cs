using System;
using System.Collections.Generic;
using UnityEngine;

public class FishesSpawner : MonoBehaviour
{
    public static FishesSpawner instance;
    [SerializeField] private FishesList fishesList;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private int layersPerBiome = 4; //Quanti layer compongono un bioma
    [SerializeField] private List<int> fishesOnLayerPerBiome; //Per ogni bioma, quanti pesci sono presenti in un layer
    [SerializeField] private List<int> fishesOnLayerPerBiomeWithLowAffection;
    [SerializeField] private List<int> fishesOnLayerPerBiomeWithGrandson; //Per ogni bioma, quanti pesci sono presenti in un layer se è presente il nipote
    [Tooltip("Usato solo se la camera non è ortografica: unità visibili in verticale.")]
    [SerializeField] private float unitsVisibleToCamera = 11; //Unità comprese nel campo visivo verticale della camera
    [Tooltip("Layer extra tenuti attivi sopra e sotto la parte visibile, così i pesci spariscono/compaiono sempre fuori dallo schermo.")]
    [SerializeField, Min(0)] private int extraLayersMargin = 1;
    [SerializeField] private Vector2 fishesPositionXRange;
    private float layersSize = 5f; //Altezza di un layer
    private List<Dictionary<string, int>> fishesInLayers; //Quantità di pesci specifici per ogni layer
    private Dictionary<string, int> totalFishes; //Quantità totale di ogni tipo di pesce nel mare
    private int currentLayer = int.MinValue; //Layer in cui si trova la camera
    private int layersVisible = 3; //Quanti layer tenere attivi sopra e sotto il currentLayer
    private List<Dictionary<string, List<GameObject>>> fishesActivatedPerLayer; //Pesci attivi per ogni layer
    private bool[] layerActive; //Quali layer hanno i pesci attivi in questo momento

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (fishesInLayers == null)
            return;

        int cameraLayer = GetLayerAt(cameraTransform.position.y);
        if (cameraLayer != currentLayer)
        {
            currentLayer = cameraLayer;
            RefreshActiveLayers();
        }
    }

    public void Initialize()
    {
        int biomes = FishingManager.instance.GetBiomesUnlocked();
        int layers = biomes * layersPerBiome + 1;
        layersSize = FishingManager.instance.biomesHeight / layersPerBiome;
        layersVisible = ComputeLayersToKeepActive();
        fishesInLayers = new(new Dictionary<string, int>[layers]);
        totalFishes = new();
        fishesActivatedPerLayer = new(new Dictionary<string, List<GameObject>>[layers]);
        layerActive = new bool[layers];
        List<FishPercentages> fishesPercentagePerBiomes;
        if (GameManager.instance.IsGrandsonHere() || GameManager.instance.GetCurrentScene() != Scene.Fishing)
            fishesPercentagePerBiomes = fishesList.fishesRatesPerBiome;
        else
            fishesPercentagePerBiomes = fishesList.fishesRatesPerBiomeWithoutGrandson;
        int y = -1;
        Dictionary<string, int> fishesInLayer = null;
        List<int> fishesOnLayerPerBiome;
        if (GameManager.instance.IsGrandsonHere())
        {
            if (GameManager.instance.GetAffection() >= -5)
                fishesOnLayerPerBiome = fishesOnLayerPerBiomeWithGrandson;
            else
                fishesOnLayerPerBiome = fishesOnLayerPerBiomeWithLowAffection;
        }
        else
        {
            fishesOnLayerPerBiome = this.fishesOnLayerPerBiome;
        }
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

        // Attiva subito i layer giusti per la posizione attuale della camera.
        currentLayer = GetLayerAt(cameraTransform.position.y);
        RefreshActiveLayers();
    }

    /// <summary>
    /// Quanti layer tenere attivi sopra e sotto quello della camera.
    /// Calcolato dalla camera vera (metà altezza visibile), più un margine di sicurezza:
    /// prima un valore troppo piccolo faceva sparire i pesci ancora visibili ai bordi dello schermo.
    /// </summary>
    private int ComputeLayersToKeepActive()
    {
        float halfHeight = unitsVisibleToCamera / 2f;
        Camera cam = cameraTransform != null ? cameraTransform.GetComponent<Camera>() : null;
        if (cam != null && cam.orthographic)
            halfHeight = cam.orthographicSize;
        return Mathf.CeilToInt(halfHeight / layersSize) + extraLayersMargin;
    }

    private int GetLayerAt(float worldY)
    {
        return Mathf.FloorToInt(-worldY / layersSize);
    }

    /// <summary>
    /// Allinea i layer attivi alla finestra [currentLayer - layersVisible, currentLayer + layersVisible].
    /// Ogni layer viene acceso o spento solo se serve: un layer non può essere attivato due volte
    /// (perdendo traccia dei suoi pesci) né spento due volte, e funziona anche se la camera
    /// salta di più layer in un colpo solo (es. quando torna in superficie).
    /// </summary>
    private void RefreshActiveLayers()
    {
        int min = currentLayer - layersVisible;
        int max = currentLayer + layersVisible;

        // Prima spegni (così i pesci tornano nel pool), poi accendi.
        for (int layer = 0; layer < layerActive.Length; layer++)
        {
            if (layerActive[layer] && (layer < min || layer > max))
                DeactivateFishesOnLayer(layer);
        }
        for (int layer = 0; layer < layerActive.Length; layer++)
        {
            if (!layerActive[layer] && layer >= min && layer <= max)
                ActivateFishesOnLayer(layer);
        }
    }

    private Vector2 GetRandomPositionInLayer(int layer)
    {
        return new Vector2(UnityEngine.Random.Range(fishesPositionXRange.x, fishesPositionXRange.y), UnityEngine.Random.Range(layer * -layersSize, (layer + 1) * -layersSize));
    }

    private void ActivateFishesOnLayer(int layer)
    {
        if (fishesActivatedPerLayer[layer] == null)
            fishesActivatedPerLayer[layer] = new();

        foreach (var kvp in fishesInLayers[layer])
        {
            List<GameObject> fishes = new();
            for (int j = 0; j < kvp.Value; j++)
            {
                float maxY = -layer * layersSize;
                if (layer == 0)
                    maxY -= 1f;
                fishes.Add(FishesPool.instance.ActivateFish(GetRandomPositionInLayer(layer), kvp.Key, new Vector2(fishesPositionXRange.x, fishesPositionXRange.y), new Vector2(-(layer + 1) * layersSize, maxY), layer));
            }
            fishesActivatedPerLayer[layer][kvp.Key] = fishes;
        }
        layerActive[layer] = true;
    }

    public void RemoveFishFromFishesActivatedPerLayer(int layer, string type, GameObject fish)
    {
        if (layer < 0 || layer >= fishesActivatedPerLayer.Count)
            return;
        if (fishesActivatedPerLayer[layer] != null && fishesActivatedPerLayer[layer].TryGetValue(type, out List<GameObject> fishes))
            fishes.Remove(fish);
    }

    private void DeactivateFishesOnLayer(int layer)
    {
        if (fishesActivatedPerLayer[layer] != null)
        {
            foreach (var kvp in fishesActivatedPerLayer[layer])
            {
                for (int j = 0; j < kvp.Value.Count; j++)
                {
                    FishesPool.instance.DeactivateFish(kvp.Value[j], kvp.Key);
                }
                // Svuota la lista: quei pesci ora sono nel pool e potrebbero essere riusati in un altro layer.
                kvp.Value.Clear();
            }
        }
        layerActive[layer] = false;
    }
}
