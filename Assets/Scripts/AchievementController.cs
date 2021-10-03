using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementController : MonoBehaviour
{
    // Instance ini mirip seperti pada GameManager, fungsinya adalah membuat sistem singleton
    // untuk memudahkan pemanggilan script yang bersifat manager dari script lain
    private static AchievementController _instance = null;
    public static AchievementController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AchievementController>();
            }
            return _instance;
        }
    }

    [SerializeField] private Transform _popUpTransform;
    [SerializeField] private Text _popUpText;
    [SerializeField] private float _popUpShowDuration = 3f;
    [SerializeField] private List<AchievementData> _achievementList;

    private float _popUpShowDurationCounter;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_popUpShowDurationCounter > 0)
        {
            // kurangi durasi ketika pop up durasi lebih dari 0?
            _popUpShowDurationCounter -= Time.unscaledDeltaTime;
            // lerp = mengubah value secara perlahan
            _popUpTransform.localScale = Vector3.LerpUnclamped(_popUpTransform.localScale, Vector3.one, 0.5f);
            // apa ini gan
        }
        else
        {
            _popUpTransform.localScale = Vector2.LerpUnclamped(_popUpTransform.localScale, Vector2.right, 0.5f);
        }
    }

    public void UnlockAchievement(AchievementType type, string value)
    {
        //Debug.Log(type.ToString());
        AchievementData achievement=null;
        // penanda jika achievementnya gold atau bukan
        bool isGold = false;
        // loop untuk list achievement
        for (var i = 0; i < _achievementList.Count; i++)
        {
            string achievementType = Convert.ToString(_achievementList[i].Type);
            string curType = Convert.ToString(type);

            // cek apakah achievement index sekarang sama2 berjenis reach gold
            if (achievementType.Equals("ReachGold", StringComparison.OrdinalIgnoreCase) && 
                curType.Equals("ReachGold", StringComparison.OrdinalIgnoreCase)/*int.TryParse(value, out n1)*/)
            {
                // jika iya, cek mana yang lebih besar antara totalGold atau achievement
                double max = Math.Max(Double.Parse(value), Double.Parse(_achievementList[i].Value));
                // Debug.Log(max);
                // jika nilai gold sekarang lebih besar dan achievement belum unlock
                if (max == Double.Parse(value) && _achievementList[i] !=null && !_achievementList[i].IsUnlocked)
                {
                    achievement = _achievementList.Find(a => a.Type == type && a.Value == _achievementList[i].Value);
                    isGold = true;
                }
            }
        }
        // mencari data achievement
        if (!isGold)
        {
            achievement = _achievementList.Find(a => a.Type == type && a.Value == value);
        }

        if (achievement != null && !achievement.IsUnlocked)
        {
            achievement.IsUnlocked = true;
            ShowAchievementPopUp(achievement);
        }
    }

    private void ShowAchievementPopUp(AchievementData achievement)
    {
        _popUpText.text = achievement.Title;
        _popUpShowDurationCounter = _popUpShowDuration;
        _popUpTransform.localScale = Vector2.right;
    }
}

// serializa script

[System.Serializable]

public class AchievementData
{
    public string Title;
    public AchievementType Type;
    public string Value;
    public bool IsUnlocked;
}

public enum AchievementType
{
    UnlockResource,
    ReachGold
}
