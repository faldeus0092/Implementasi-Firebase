using UnityEngine;
using UnityEngine.UI;

public class ResourceController : MonoBehaviour
{
    public Text ResourceDescription;
    public Text ResourceUpgradeCost;
    public Text ResourceUnlockCost;
    private ResourceConfig _config;
    private int _level
    {
        set
        {
            // menyimpan value yang diset pada varuabel ini pada progress data, lalu save
            UserDataManager.Progress.ResourcesLevels[_index] = value;
            UserDataManager.Save(true);
        }
        get
        {
            // jika tidak ada data
            if (!UserDataManager.HasResources(_index))
            {
                // level 1
                return 1;
            }
            // jika ada, tampilkan sesuai level player
            return UserDataManager.Progress.ResourcesLevels[_index];
        }

    }
    public Button ResourceButton;
    public Image ResourceImage;
    private int _index;

    public void SetConfig(int index, ResourceConfig config)

    {
        _index = index;
        _config = config;

        // ToString("0") berfungsi untuk membuang angka di belakang koma
        ResourceDescription.text = $"{ _config.Name } Lv. { _level }\n+{ GetOutput().ToString("0") }";
        ResourceUnlockCost.text = $"Unlock Cost\n{ _config.UnlockCost }";
        ResourceUpgradeCost.text = $"Upgrade Cost\n{ GetUpgradeCost() }";

        // ??
        SetUnlocked(_config.UnlockCost == 0 || UserDataManager.HasResources(_index));
    }



    public double GetOutput()
    {
        return _config.Output * _level;
    }



    public double GetUpgradeCost()
    {
        return _config.UpgradeCost * _level;
    }

    public void UpgradeLevel()
    {
        double upgradeCost = GetUpgradeCost();
        if(UserDataManager.Progress.Gold < upgradeCost)
        {
            // insufficient
            return;
        }
        GameManager.Instance.AddGold(-upgradeCost);
        _level++;
        ResourceUpgradeCost.text = $"Upgrade Cost\n{GetUpgradeCost()}";
        ResourceDescription.text = $"{_config.Name} Lv.{_level}\n+{GetOutput().ToString("0")}";

        // log juga setiap kali upgrade sesuatu
        AnalyticsManager.LogUpgradeEvent(_index, _level);
    }


    public double GetUnlockCost()
    {
        return _config.UnlockCost;
    }

    public bool isUnlocked { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        // listener untuk resource button
        ResourceButton.onClick.AddListener(()=> {
            if (isUnlocked)
            {
                UpgradeLevel();
            }
            else
            {
                UnlockResource();
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UnlockResource()
    {
        double unlockCost = GetUnlockCost();
        if(UserDataManager.Progress.Gold < unlockCost)
        {
            return;
        }
        SetUnlocked(true);
        GameManager.Instance.ShowNextResource();
        AchievementController.Instance.UnlockAchievement(AchievementType.UnlockResource, _config.Name);

        // log setiap kali unlock resource
        AnalyticsManager.LogUnlockEvent(_index);
    }

    public void SetUnlocked(bool unlocked)
    {
        isUnlocked = unlocked;

        if (unlocked)
        {
            // jika resources baru diunlock dan tidak ada dalam progress data, maka simpan data
            if (!UserDataManager.HasResources(_index))
            {
                UserDataManager.Progress.ResourcesLevels.Add(_level);
                UserDataManager.Save(true);
            }
        }

        ResourceImage.color = isUnlocked ? Color.white : Color.gray;
        ResourceUnlockCost.gameObject.SetActive(!unlocked);
        ResourceUpgradeCost.gameObject.SetActive(unlocked);
    }
}
