using System.Collections;
using TMPro;
using UnityEngine;

public class DissolveText : MonoBehaviour
{
    [SerializeField] private float timeToLive = 0.5f;
    [SerializeField] private float fadeDuration = 0.5f;
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        yield return new WaitForSeconds(timeToLive);
        float elapsedTime = 0;
        float startingSize = rectTransform.localScale.x;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float scale = Mathf.Lerp(startingSize, 0, elapsedTime / fadeDuration);
            rectTransform.localScale = new Vector3(scale, scale, 1);
            yield return null;
        }
        Destroy(gameObject);
    }
}
