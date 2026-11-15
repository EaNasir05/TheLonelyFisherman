using UnityEngine;
using UnityEngine.EventSystems;

public class ShopButtonWithoutUpgrade : MonoBehaviour, ISelectHandler
{
    public void OnSelect(BaseEventData data)
    {
        UIManager.instance.HideUpgradeInfo();
    }
}
