using UnityEngine;

public class TutorialVillage : MonoBehaviour
{
    [SerializeField] private Collider2D shopTrigger;
    [SerializeField] private Collider2D villageTrigger;
    [SerializeField] private GameObject colliderHut;
    [SerializeField] private GameObject colliderFishing;

    private void Start()
    {
        if (GameManager.instance.GetDay() == 0 && GameManager.instance.GetCurrentScene() == Scene.Village)
        {
            colliderHut.SetActive(true);
            UIManager.instance.HideCloseShopButton();
            UIManager.instance.GetStartingNodeShop().GetComponent<UpgradeButton>().onPurchase += Tutorial1;
        }
        else if (GameManager.instance.GetDay() == 1)
        {
            if (GameManager.instance.GetCurrentScene() == Scene.Village)
            {
                if (GameManager.instance.GetBoatLevel() != 0)
                shopTrigger.enabled = false;
                colliderFishing.SetActive(true);
            }
            else
            {
                villageTrigger.enabled = false;
            }
        }
    }

    private void Tutorial1()
    {
        DialoguesManager.instance.StartDialogue("ShopTutorial3");
    }
}
