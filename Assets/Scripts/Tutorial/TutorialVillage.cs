using UnityEngine;

public class TutorialVillage : MonoBehaviour
{
    [SerializeField] private Collider2D triggerToBlockBoat;
    [SerializeField] private Collider2D triggerToBlockVillage;
    [SerializeField] private GameObject colliderToSpawnVillage;
    [SerializeField] private GameObject colliderToSpawnBoat;

    private void Start()
    {
        if (GameManager.instance.GetDay() == 0 && GameManager.instance.GetCurrentScene() == Scene.Village)
        {
            triggerToBlockVillage.enabled = false;
            colliderToSpawnVillage.SetActive(true);
            UIManager.instance.HideCloseShopButton();
        }
        else if (GameManager.instance.GetDay() == 1)
        {
            if (GameManager.instance.GetCurrentScene() == Scene.Village)
            {
                colliderToSpawnBoat.SetActive(true);
            }
            else
            {
                triggerToBlockBoat.enabled = false;
            }
        }
    }
}
