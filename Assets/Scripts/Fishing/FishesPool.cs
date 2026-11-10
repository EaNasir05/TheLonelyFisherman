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
                GameObject fish = Instantiate(fishesList.GetFishPrefabByType(fishType));
                fish.SetActive(false);
                fish.transform.parent = fishesParent;
                queue.Enqueue(fish);
            }
            pool.Add(fishType, queue);
        }
    }

    public GameObject ActivateFish(Vector2 position, string fishType)
    {
        GameObject fish = pool[fishType].Dequeue();
        fish.transform.position = position;
        fish.SetActive(true);
        return fish;
    }

    public void DeactivateFish(GameObject fish, string fishType)
    {
        fish.SetActive(false);
        pool[fishType].Enqueue(fish);
    }
}
