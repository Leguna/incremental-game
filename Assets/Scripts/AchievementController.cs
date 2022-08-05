using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementController : MonoBehaviour
{
    private static AchievementController _instance;
    [SerializeField] private Transform popUpTransform;
    [SerializeField] private Text popUpText;
    [SerializeField] private float popUpShowDuration = 3f;
    [SerializeField] private List<AchievementData> achievementList;
    private float _popUpShowDurationCounter;

    public static AchievementController Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<AchievementController>();
            return _instance;
        }
    }

    private void Update()
    {
        if (_popUpShowDurationCounter > 0)
        {
            _popUpShowDurationCounter -= Time.unscaledDeltaTime;
            popUpTransform.localScale = Vector3.LerpUnclamped(popUpTransform.localScale, Vector3.one, 0.5f);
        }
        else
        {
            popUpTransform.localScale = Vector2.LerpUnclamped(popUpTransform.localScale, Vector3.right, 0.5f);
        }
    }

    public void UnlockAchievement(AchievementType type, string value)
    {
        var achievement = achievementList.Find(a => a.type == type && a.value == value);
        if (achievement != null && !achievement.isUnlocked)
        {
            achievement.isUnlocked = true;
            ShowAchivementPopUp(achievement);
        }
    }

    private void ShowAchivementPopUp(AchievementData achievement)
    {
        popUpText.text = achievement.title;
        _popUpShowDurationCounter = popUpShowDuration;
        popUpTransform.localScale = Vector2.right;
    }
}

[Serializable]
public class AchievementData
{
    public string title;
    public AchievementType type;
    public string value;
    public bool isUnlocked;
}

public enum AchievementType
{
    UnlockResource
}
