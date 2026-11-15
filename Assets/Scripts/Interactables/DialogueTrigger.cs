using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private string dialogueNode = "";
    [SerializeField] private GameObject inputIcon;
    [SerializeField] private SpriteRenderer interactableRenderer;
    [SerializeField] private Color interactableColor = Color.yellow;
    private InputHandler inputHandler;
    private bool playerHere = false;

    private void Start()
    {
        inputHandler = FindFirstObjectByType<InputHandler>();
        inputHandler.onInteractInput += StartDialogue;
    }

    private void OnDestroy()
    {
        inputHandler.onInteractInput -= StartDialogue;
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

    private void StartDialogue()
    {
        if (playerHere)
            DialoguesManager.instance.StartDialogue(dialogueNode);
    }
}
