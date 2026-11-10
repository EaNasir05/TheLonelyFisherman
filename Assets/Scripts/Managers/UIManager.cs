using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum Scene { Island, Boat, Fishing, Ending, Lighthouse, Hut }

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] private Scene currentScene;

    [Header("General")]
    [SerializeField] private Image blackScreen;

    [Header("Island")]
    [SerializeField] private GameObject shopMenu;

    [Header("Fishing")]
    [SerializeField] private TMP_Text fishingPoints;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Image staminaFillImage;
    [SerializeField] private Color staminaFillColor;
    [SerializeField] private Color weightFillColor;
    [SerializeField] private Image staminaBackgroundImage;
    [SerializeField] private Color staminaBackgroundColor;
    [SerializeField] private Color weightBackgroundColor;
    private Hook hookScript;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        hookScript = FindFirstObjectByType<Hook>();
    }

    public void ShowStaminaBar()
    {
        staminaSlider.gameObject.SetActive(true);
    }

    public void HideStaminaBar()
    {
        staminaSlider.gameObject.SetActive(false);
    }

    public void IncreaseStaminaBarScale()
    {
        StartCoroutine(ScaleInStamina());
    }

    public void ResetStaminaBarScale()
    {
        StartCoroutine(ScaleOutStamina());
    }

    public IEnumerator StaminaBarAnimationOnOverweight()
    {
        //Animazione di sovraccarico
        yield return null;
    }

    private IEnumerator ScaleInStamina()
    {
        //Aumenta la scale
        yield return null;
    }

    private IEnumerator ScaleOutStamina()
    {
        //Resetta la scale
        yield return null;
    }

    public void RefreshStaminaBar()
    {
        staminaSlider.maxValue = hookScript.maxStamina;
        staminaSlider.value = hookScript.maxStamina;
        staminaBackgroundImage.color = staminaBackgroundColor;
        staminaFillImage.color = staminaFillColor;
    }

    public void RefreshWeightBar()
    {
        staminaSlider.maxValue = hookScript.GetMaxWeight();
        staminaSlider.value = 0f;
        staminaBackgroundImage.color = weightBackgroundColor;
        staminaFillImage.color = weightFillColor;
    }

    public void UpdateStaminaBar(float value)
    {
        staminaSlider.value = value;
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
