using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishesPool : MonoBehaviour
{
    public static FishesPool instance;
    [SerializeField] private FishesList fishesList;
    [SerializeField] private Transform fishesParent;
    private Dictionary<string, Queue<GameObject>> pool;

    private void Awake()
    {
        instance = this;
    }

    public void Initialize(Dictionary<string, int> fishesPerType)
    {
        pool = new Dictionary<string, Queue<GameObject>>();
        foreach (var kvp in fishesPerType)
        {
            string fishType = kvp.Key;
            Queue<GameObject> queue = new();
            for (int i = 0; i < kvp.Value; i++)
            {
                queue.Enqueue(CreateFish(fishType));
            }
            pool.Add(fishType, queue);
        }
    }

    private GameObject CreateFish(string fishType)
    {
        GameObject fish = Instantiate(fishesList.GetFishPrefabByType(fishType));
        fish.SetActive(false);
        fish.transform.parent = fishesParent;
        return fish;
    }

    /// <summary>
    /// Prende un pesce dal pool. Se il pool di quel tipo è vuoto ne crea uno nuovo invece di dare errore.
    /// Succede quando si risale e si riattiva un layer mentre alcuni pesci di quel tipo sono ancora
    /// attaccati all'amo (pescati, ma non ancora restituiti al pool).
    /// </summary>
    private GameObject TakeFish(string fishType)
    {
        if (!pool.TryGetValue(fishType, out Queue<GameObject> queue))
        {
            queue = new Queue<GameObject>();
            pool.Add(fishType, queue);
        }

        while (queue.Count > 0)
        {
            GameObject fish = queue.Dequeue();
            if (fish != null)
                return fish;
        }

        return CreateFish(fishType);
    }

    public GameObject ActivateFish(Vector2 position, string fishType)
    {
        GameObject fish = TakeFish(fishType);
        fish.transform.position = position;
        fish.SetActive(true);
        return fish;
    }

    public GameObject ActivateFish(Vector2 position, string fishType, Vector2 movementRangeX, Vector2 movementRangeY, int layer)
    {
        GameObject fish = TakeFish(fishType);
        fish.transform.position = position;
        FishController controller = fish.GetComponent<FishController>();
        controller?.SetMovementRange(movementRangeX, movementRangeY);
        controller?.SetLayer(layer);
        fish.SetActive(true);
        return fish;
    }

    public void DeactivateFish(GameObject fish, string fishType)
    {
        // Un pesce già disattivato è già nel pool: rimetterlo in coda creerebbe un doppione
        // (lo stesso pesce verrebbe poi usato in due posti contemporaneamente).
        if (fish == null || !fish.activeSelf)
            return;

        fish.transform.SetParent(fishesParent);
        fish.SetActive(false);

        if (!pool.TryGetValue(fishType, out Queue<GameObject> queue))
        {
            queue = new Queue<GameObject>();
            pool.Add(fishType, queue);
        }
        queue.Enqueue(fish);
    }
}
