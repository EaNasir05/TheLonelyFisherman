using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private string dialogueNode = "";
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
            playerHere = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerHere = false;
    }

    private void StartDialogue()
    {
        if (playerHere)
            DialoguesManager.instance.StartDialogue(dialogueNode);
    }
}
