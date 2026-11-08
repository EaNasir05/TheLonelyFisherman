using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private Image blackScreen;

    private void Awake()
    {
        instance = this;
    }

    private IEnumerator FadeIn()
    {
        float duration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / duration);
            blackScreen.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        blackScreen.color = new Color(0f, 0f, 0f, 1f);
    }

    private IEnumerator FadeOut()
    {
        float duration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(1f - (elapsedTime / duration));
            blackScreen.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        blackScreen.color = new Color(0f, 0f, 0f, 0f);
    }
}
