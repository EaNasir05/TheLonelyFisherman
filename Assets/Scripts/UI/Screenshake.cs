using System.Collections;
using UnityEngine;

public class Screenshake : MonoBehaviour
{
    public void TriggerShake(float intensity, float duration)
    {
        StartCoroutine(ShakeRoutine(intensity, duration));
    }

    private IEnumerator ShakeRoutine(float intensity, float duration)
    {
        float posX = transform.localPosition.x;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float randomX = Random.Range(-1f, 1f) * intensity;
            transform.localPosition = new Vector2(posX + randomX, transform.position.y);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
