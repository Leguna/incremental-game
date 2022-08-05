using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    [Range(0f, 1f)] public float autoCollectPercentage = 0.1f;
    public ResourceConfig[] resourcesConfigs;
    public Sprite[] resourcesSprites;
    public Transform resourcesParent;
    public ResourceController resourcePrefab;
    public Text goldInfo;
    public Text autoCollectInfo;
    public Transform coinIcon;
    public TapText tapTextPrefab;
    public double totalGold;
    private readonly List<ResourceController> _activeResources = new();
    private readonly List<TapText> _tapTextPool = new();
    private float _collectSecond;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<GameManager>();
            return _instance;
        }
    }

    private void Start()
    {
        AddAllResources();
    }

    private void Update()
    {
        _collectSecond += Time.unscaledDeltaTime;
        if (_collectSecond >= 1f)
        {
            CollectPerSecond();
            _collectSecond = 0f;
        }

        CheckResourceCost();
        coinIcon.transform.localScale = Vector3.LerpUnclamped(coinIcon.transform.localScale, Vector3.one * 2f, 0.15f);
        coinIcon.transform.Rotate(0f, 0f, Time.deltaTime * -100f);
    }

    private void AddAllResources()
    {
        var showResources = true;
        foreach (var config in resourcesConfigs)
        {
            var obj = Instantiate(resourcePrefab.gameObject, resourcesParent, false);
            var resource = obj.GetComponent<ResourceController>();
            resource.SetConfig(config);
            obj.gameObject.SetActive(showResources);
            if (showResources && !resource.IsUnlocked) showResources = false;
            _activeResources.Add(resource);
        }
    }

    public void ShowNextResource()
    {
        foreach (var resource in _activeResources)
            if (!resource.gameObject.activeSelf)
            {
                resource.gameObject.SetActive(true);
                break;
            }
    }

    private void CheckResourceCost()
    {
        foreach (var resource in _activeResources)
        {
            var isBuyable = false;
            if (resource.IsUnlocked) isBuyable = totalGold >= resource.GetUpgradeCost();
            else isBuyable = totalGold >= resource.GetUnlockCost();
            resource.resourceImage.sprite = resourcesSprites[isBuyable ? 1 : 0];
        }
    }

    private void CollectPerSecond()
    {
        double output = 0;
        foreach (var resource in _activeResources)
            if (resource.IsUnlocked)
                output += resource.GetOutput();

        output *= autoCollectPercentage;
        autoCollectInfo.text = $"Auto Collect: {output:F1} / second";
        AddGold(output);
    }

    public void AddGold(double value)
    {
        totalGold += value;
        goldInfo.text = $"Gold: {totalGold:0}";
    }

    public void CollectByTap(Vector3 tapPosition, Transform parent)
    {
        double output = 0;

        foreach (var resource in _activeResources)
            if (resource.IsUnlocked)
                output += resource.GetOutput();

        var tapText = GetOrCreateTapText();
        Transform transform1;
        (transform1 = tapText.transform).SetParent(parent, false);
        transform1.position = tapPosition;
        tapText.text.text = $"+{output:0}";
        tapText.gameObject.SetActive(true);
        coinIcon.transform.localScale = Vector3.one * 1.75f;
        AddGold(output);
    }

    private TapText GetOrCreateTapText()
    {
        var tapText = _tapTextPool.Find(t => !t.gameObject.activeSelf);
        if (tapText == null)
        {
            tapText = Instantiate(tapTextPrefab).GetComponent<TapText>();
            _tapTextPool.Add(tapText);
        }

        return tapText;
    }
}

[Serializable]
public struct ResourceConfig
{
    public string name;
    public double unlockCost;
    public double upgradeCost;
    public double output;
}
