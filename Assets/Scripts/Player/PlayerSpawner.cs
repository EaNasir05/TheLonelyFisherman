using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner instance;

    [SerializeField] private Transform spawnPosFromFishing;
    [SerializeField] private Transform spawnPosFromBoat;
    [SerializeField] private Transform spawnPosFromVillage;
    [SerializeField] private Transform playerTransform;

    private void Awake()
    {
        instance = this;
    }

    public void SpawnPlayerFromFishing()
    {
        playerTransform.position = spawnPosFromFishing.position;
    }

    public void SpawnPlayerFromBoat()
    {
        playerTransform.position = spawnPosFromBoat.position;
    }

    public void SpawnPlayerFromVillage()
    {
        playerTransform.position = spawnPosFromVillage.position;
    }
}
