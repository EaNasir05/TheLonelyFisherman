using UnityEngine;

public class ShopTrigger : MonoBehaviour
{
    [SerializeField] private GameObject inputIcon;
    [SerializeField] private SpriteRenderer interactableRenderer;
    [SerializeField] private Color interactableColor = Color.yellow;
    private InputHandler inputHandler;
    private bool playerHere = false;

    private void Start()
    {
        inputHandler = FindFirstObjectByType<InputHandler>();
        inputHandler.onInteractInput += OpenShop;
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
            inputIcon?.SetActive(false);
        }
    }

    private void OpenShop()
    {
        if (playerHere)
            UIManager.instance.OpenShop();
    }
}
