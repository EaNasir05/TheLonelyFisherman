using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum Scene { Island, Boat, Fishing, Ending, Lighthouse, Hut, Menu }

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
    private RectTransform staminaRectTransform;
    [SerializeField] private Image staminaFillImage;
    [SerializeField] private Color staminaFillColor;
    [SerializeField] private Color weightFillColor;
    [SerializeField] private Image staminaBackgroundImage;
    [SerializeField] private Color staminaBackgroundColor;
    [SerializeField] private Color weightBackgroundColor;
    [SerializeField] private float staminaMaxScale;
    [SerializeField] private float staminaScalingDuration;
    private Coroutine scalingRoutine;
    [SerializeField] private Transform hookCanvas;
    [SerializeField] private GameObject pointsGainTextPrefab;
    [SerializeField] private Vector2 scaleRangePointsGainText;
    [SerializeField] private Vector2 valueRangePointsGainText;
    [SerializeField] private float maxDistanceFromHookToText;
    private Hook hookScript;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        hookScript = FindFirstObjectByType<Hook>();
        staminaRectTransform = staminaSlider?.GetComponent<RectTransform>();
        currentScene = GameManager.instance.GetCurrentScene();
    }

    public void UpdatePointsText(float value)
    {
        fishingPoints.text = value.ToString();
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
        scalingRoutine = StartCoroutine(ScaleInStamina());
    }

    public void ResetStaminaBarScale()
    {
        scalingRoutine = StartCoroutine(ScaleOutStamina());
    }

    public IEnumerator StaminaBarAnimationOnOverweight()
    {
        //Animazione di sovraccarico
        yield return null;
    }

    private IEnumerator ScaleInStamina()
    {
        if (scalingRoutine != null)
            StopCoroutine(scalingRoutine);
        float elapsedTime = 0;
        float startingScale = staminaRectTransform.localScale.x;
        float targetScale = staminaMaxScale;
        float realDuration = Mathf.Lerp(staminaScalingDuration, 0, (startingScale - 1) / (targetScale - 1));
        while (elapsedTime < realDuration)
        {
            elapsedTime += Time.deltaTime;
            float scale = Mathf.Lerp(startingScale, targetScale, elapsedTime / realDuration);
            staminaRectTransform.localScale = new Vector3(scale, scale, 1);
            yield return null;
        }
        scalingRoutine = null;
    }

    private IEnumerator ScaleOutStamina()
    {
        if (scalingRoutine != null)
            StopCoroutine(scalingRoutine);
        float elapsedTime = 0;
        float startingScale = staminaRectTransform.localScale.x;
        float targetScale = 1;
        float realDuration = Mathf.Lerp(staminaScalingDuration, 0, (staminaMaxScale - startingScale) / (staminaMaxScale - 1));
        while (elapsedTime < realDuration)
        {
            elapsedTime += Time.deltaTime;
            float scale = Mathf.Lerp(startingScale, targetScale, elapsedTime / realDuration);
            staminaRectTransform.localScale = new Vector3(scale, scale, 1);
            yield return null;
        }
        scalingRoutine = null;
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

    public void PrintPointGainText(float value)
    {
        Transform pgt = Instantiate(pointsGainTextPrefab).transform;
        pgt.SetParent(hookCanvas);
        pgt.localPosition = new Vector2(Random.Range(-maxDistanceFromHookToText, maxDistanceFromHookToText), Random.Range(-maxDistanceFromHookToText, maxDistanceFromHookToText));
        pgt.GetComponent<TMP_Text>().text = "+" + value;
        float fixedValue = Mathf.Clamp(value, scaleRangePointsGainText.x, scaleRangePointsGainText.y);
        float scale = Mathf.Lerp(scaleRangePointsGainText.x, scaleRangePointsGainText.y, fixedValue - scaleRangePointsGainText.x / scaleRangePointsGainText.y - scaleRangePointsGainText.x);
        pgt.localScale = new Vector3(scale, scale, 1);
    }

    public void PrintPointLossText(float value)
    {
        Transform pgt = Instantiate(pointsGainTextPrefab).transform;
        pgt.SetParent(hookCanvas);
        pgt.localPosition = new Vector2(Random.Range(-maxDistanceFromHookToText, maxDistanceFromHookToText), Random.Range(-maxDistanceFromHookToText, maxDistanceFromHookToText));
        TMP_Text tmp = pgt.GetComponent<TMP_Text>();
        tmp.text = "-" + value;
        tmp.color = Color.red;
        pgt.localScale = new Vector3(2, 2, 1);
    }

    public IEnumerator FadeOut(float duration, string sceneName)
    {
        blackScreen.gameObject.SetActive(true);
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / duration);
            blackScreen.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }
        blackScreen.color = new Color(0f, 0f, 0f, 1f);

        SceneManager.LoadScene(sceneName);
    }

    public IEnumerator FadeIn(float duration)
    {
        blackScreen.gameObject.SetActive(true);
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(1f - (elapsedTime / duration));
            blackScreen.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }
        blackScreen.color = new Color(0f, 0f, 0f, 0f);
        blackScreen.gameObject.SetActive(false);
    }
}
