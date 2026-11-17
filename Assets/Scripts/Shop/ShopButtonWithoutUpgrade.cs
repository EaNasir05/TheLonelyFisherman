using UnityEngine;
using UnityEngine.EventSystems;

public class ShopButtonWithoutUpgrade : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{
    private EventSystem eventSystem;
    private UpgradesList upgradesList;

    private void Start()
    {
        eventSystem = FindFirstObjectByType<EventSystem>();
    }

    public void OnSelect(BaseEventData data)
    {
        UIManager.instance.HideUpgradeInfo();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        eventSystem.SetSelectedGameObject(gameObject);
    }

    public void CloseShop()
    {
        if (upgradesList.GetUpgradeByName("Attrezzi da carpentiere").unlocked)
            UIManager.instance.CloseShop();
    }
}
