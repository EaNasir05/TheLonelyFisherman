using System.Collections;
using UnityEngine;

public class WaitAndDestroy : MonoBehaviour
{
    [SerializeField] private float timeToLive = 0.5f;

    private void Start()
    {
        StartCoroutine(DestroyAfterBirth());
    }

    private IEnumerator DestroyAfterBirth()
    {
        yield return new WaitForSeconds(timeToLive);
        Destroy(gameObject);
    }
}