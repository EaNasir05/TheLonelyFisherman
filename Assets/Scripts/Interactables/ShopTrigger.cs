using UnityEngine;

public class ShopTrigger : MonoBehaviour
{
    [SerializeField] private string nodeName = "Shop";
    [SerializeField] private GameObject inputIcon;
    [SerializeField] private SpriteRenderer interactableRenderer;
    [SerializeField] private Color interactableColor = Color.yellow;
    private InputHandler inputHandler;
    private bool playerHere = false;
    private bool shopping = false;

    public void SetShopping(bool value) => shopping = value;

    private void Start()
    {
        inputHandler = FindFirstObjectByType<InputHandler>();
        inputHandler.onInteractInput += OpenShop;
        UIManager.instance.onShopInteraction += SetShopping;
    }

    private void OnDestroy()
    {
        inputHandler.onInteractInput -= OpenShop;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerHere = true;
            if (interactableRenderer != null)
                interactableRenderer.color = interactableColor;
            if (inputIcon != null)
                inputIcon?.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerHere = false;
            if (interactableRenderer != null)
                interactableRenderer.color = Color.white;
            if (inputIcon != null)
                inputIcon?.SetActive(false);
        }
    }

    public void OpenShop()
    {
        if (playerHere && !shopping)
        {
            UIManager.instance.OpenShop();
            DialoguesManager.instance.StartDialogue(nodeName);
        }
    }
}
