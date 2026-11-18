using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("General")]
    [SerializeField] private Image blackScreen;
    [SerializeField] private GameObject happyMoodObject;
    [SerializeField] private GameObject sadMoodObject;
    public Action onFadeIn;
    private EventSystem eventSystem;

    [Header("Village")]
    [SerializeField] private GrandsonController grandsonController;
    [SerializeField] private TMP_Text moneyCount;
    [SerializeField] private GameObject shopMenu;
    [SerializeField] private GameObject startingNodeShop;
    [SerializeField] private GameObject upgradeInfo;
    [SerializeField] private Image upgradeIcon;
    [SerializeField] private TMP_Text upgradeName;
    [SerializeField] private TMP_Text upgradeDescription;
    [SerializeField] private TMP_Text upgradeCost;
    [SerializeField] private GameObject upgradeUnlocked;
    [SerializeField] private TMP_Text upgradeEffect;
    [SerializeField] private GameObject closeShopButton;
    public Action<bool> onShopInteraction;

    [Header("Fishing")]
    [SerializeField] private GameObject grandson;
    [SerializeField] private TMP_Text fishingPoints;
    [SerializeField] private Slider attemptsSlider;
    [SerializeField] private GameObject resultsWindow;
    [SerializeField] private TMP_Text resultsText;
    [SerializeField] private GameObject exitSceneButton;
    [SerializeField] private Slider hookPositionSlider;
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
    [SerializeField] private GameObject overWeightEffect;
    [SerializeField] private GameObject normalHitPrefab;
    [SerializeField] private GameObject normalCritHitPrefab;
    [SerializeField] private GameObject magicHitPrefab;
    [SerializeField] private GameObject magicCritHitPrefab;
    [SerializeField] private Screenshake screenshake;
    private Hook hookScript;

    [Header("Boat")]
    [SerializeField] private Sprite[] boatSprites;
    [SerializeField] private SpriteRenderer boatRenderer;

    public GameObject GetStartingNodeShop() => startingNodeShop;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        hookScript = FindFirstObjectByType<Hook>();
        staminaRectTransform = staminaSlider?.GetComponent<RectTransform>();
        eventSystem = FindFirstObjectByType<EventSystem>();
        if (DialoguesManager.instance != null)
            DialoguesManager.instance.onDialogueActivation += HideUpgradeInfoOnTalking;
    }

    private void OnDestroy()
    {
        // Senza questo, lo UIManager di ogni scena precedente restava iscritto all'evento
        // anche dopo essere stato distrutto.
        if (DialoguesManager.instance != null)
            DialoguesManager.instance.onDialogueActivation -= HideUpgradeInfoOnTalking;
    }

    #region Grandson

    public void SetHappyMood()
    {
        happyMoodObject.SetActive(true);
        sadMoodObject.SetActive(false);
    }

    public void SetNeutralMood()
    {
        happyMoodObject.SetActive(true);
        sadMoodObject.SetActive(false);
    }

    public void SetSadMood()
    {
        happyMoodObject.SetActive(false);
        sadMoodObject.SetActive(true);
    }

    public void ShowGrandson()
    {
        grandsonController?.gameObject.SetActive(true);
        if (grandson != null)
            grandson.SetActive(true);
    }

    public void MakeGrandsonFollow()
    {
        grandsonController.state = GrandsonState.Following;
    }

    public void MakeGrandsonGoAway()
    {
        grandsonController.state = GrandsonState.Escaping;
    }

    #endregion Grandson

    #region Money

    public void UpdateMoneyCount(int value)
    {
        moneyCount?.SetText(value + "€");
    }

    #endregion Money

    #region Shop

    public void OpenShop()
    {
        upgradeInfo.SetActive(false);
        eventSystem.firstSelectedGameObject = startingNodeShop;
        shopMenu.SetActive(true);
        SFXManager.Play(SFX.ShopOpen);
        onShopInteraction?.Invoke(true);
    }

    public void CloseShop()
    {
        shopMenu.SetActive(false);
        onShopInteraction?.Invoke(false);
    }

    public void ShowCloseShopButton()
    {
        closeShopButton.SetActive(true);
    }

    public void HideCloseShopButton()
    {
        closeShopButton.SetActive(false);
    }

    public void UpdateUpgradeInfo(string name, string description, string effect, int cost, Sprite icon, bool unlocked)
    {
        upgradeName.text = name;
        upgradeDescription.text = description;
        upgradeEffect.text = effect;
        upgradeIcon.sprite = icon;
        if (unlocked)
        {
            upgradeUnlocked.SetActive(true);
            upgradeCost.gameObject.SetActive(false);
        }
        else
        {
            if (cost > GameManager.instance.GetMoney())
                upgradeCost.color = Color.red;
            else
                upgradeCost.color = Color.black;
            upgradeUnlocked.SetActive(false);
            upgradeCost.text = cost + "€";
            upgradeCost.gameObject.SetActive(true);
        }
        upgradeInfo.SetActive(true);
    }

    private void HideUpgradeInfoOnTalking(bool value)
    {
        if (value)
            HideUpgradeInfo();
    }

    public void HideUpgradeInfo()
    {
        if (upgradeInfo != null)
            upgradeInfo.SetActive(false);
    }

    #endregion Shop

    #region Fishing

    public void UpdatePointsText(float value)
    {
        fishingPoints.text = value.ToString();
    }

    public void UpdateAttemptsSlider(int maxValue, int currentValue)
    {
        attemptsSlider.maxValue = maxValue;
        attemptsSlider.value = currentValue;
    }

    public void HideAttemptsSlider()
    {
        attemptsSlider.gameObject.SetActive(false);
    }

    public void ShowFishingResults(int money)
    {
        resultsText.text = "Hai guadagnato " + money + "€!";
        SFXManager.Play(SFX.Money);
        eventSystem.SetSelectedGameObject(exitSceneButton);
        resultsWindow.SetActive(true);
    }

    public void UpdateHookPositionSlider(float currentPos, float maxPos)
    {
        hookPositionSlider.maxValue = maxPos;
        hookPositionSlider.value = currentPos;
    }

    public void UpdateHookPositionSlider(float currentPos)
    {
        hookPositionSlider.value = currentPos;
    }

    public void ShowHookPositionSlider()
    {
        hookPositionSlider.gameObject.SetActive(true);
    }

    public void HideHookPositionSlider()
    {
        hookPositionSlider.gameObject.SetActive(false);
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
        staminaSlider.maxValue = hookScript.GetMaxStamina();
        staminaSlider.value = hookScript.GetMaxStamina();
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

    public void ShowOverweightEffect()
    {
        overWeightEffect.SetActive(true);
    }

    public void HideOverweightEffect()
    {
        overWeightEffect.SetActive(false);
    }

    public void SpawnNormalHitEffect(Transform parent)
    {
        GameObject effect = Instantiate(normalHitPrefab);
        effect.transform.SetParent(parent);
        effect.transform.localPosition = Vector2.zero;
    }

    public void SpawnNormalCritHitEffect(Transform parent)
    {
        GameObject effect = Instantiate(normalCritHitPrefab);
        effect.transform.SetParent(parent);
        effect.transform.localPosition = Vector2.zero;
    }

    public void SpawnMagicHitEffect(Transform parent)
    {
        GameObject effect = Instantiate(magicHitPrefab);
        effect.transform.SetParent(parent);
        effect.transform.localPosition = Vector2.zero;
    }

    public void SpawnMagicCritHitEffect(Transform parent)
    {
        GameObject effect = Instantiate(magicCritHitPrefab);
        effect.transform.SetParent(parent);
        effect.transform.localPosition = Vector2.zero;
    }

    public void StartScreenshake(float duration, float intensity)
    {
        if (screenshake == null)
        {
            Debug.LogWarning("[UIManager] Screenshake non assegnato nell'Inspector di questo UIManager.", this);
            return;
        }
        screenshake.TriggerShake(intensity, duration);
    }
    
    #endregion Fishing

    #region Boat

    public void UpdateBoatSprite(int level)
    {
        boatRenderer.sprite = boatSprites[level];
    }

    #endregion Boat

    #region Fade

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
        
        onFadeIn?.Invoke();
    }

    #endregion Fade
}
