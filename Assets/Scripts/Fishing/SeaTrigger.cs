using UnityEngine;

public class SeaTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Hook hook = collision.GetComponent<Hook>();
            if (!hook.IsInTheSea())
            {
                hook.EnterTheSea();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Hook hook = collision.GetComponent<Hook>();
            if (hook.IsReturning())
            {
                hook.ExitTheSea();
            }
        }
    }
}
