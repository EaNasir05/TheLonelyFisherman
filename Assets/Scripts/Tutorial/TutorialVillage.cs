using UnityEngine;

public class TutorialVillage : MonoBehaviour
{
    [SerializeField] private Collider2D triggerToBlock;
    [SerializeField] private GameObject colliderToSpawn;

    private void Start()
    {
        if (triggerToBlock != null)
            triggerToBlock.enabled = false;
        colliderToSpawn?.SetActive(true);
    }
}
