using UnityEngine;
using System.Collections;
using System.Text;
using System.Threading.Tasks;

// mengatur userprogress data untuk disimpan atau diload
public static class UserDataManager
{
    private const string PROGRESS_KEY = "Progress";
    public static UserProgressData Progress;

    public static void Load()
    {
        // cek apakah ada data yang tersimpan
        if (!PlayerPrefs.HasKey(PROGRESS_KEY))
        {
            // JIKA TIDAK ADA BUAT BARU dan upload ke cloud
            Progress = new UserProgressData();
            Save(true);
        }
        else
        {
            // jika ada, overwrite dengan progress sebelumnya
            string json = PlayerPrefs.GetString(PROGRESS_KEY);
            Progress = JsonUtility.FromJson<UserProgressData>(json);
        }
    }

    public static void Save(bool uploadToCloud = false)
    {
        string json = JsonUtility.ToJson(Progress);
        PlayerPrefs.SetString(PROGRESS_KEY, json);
    }

    public static bool HasResources(int index)
    {
        return index + 1 <= Progress.ResourcesLevels.Count;
    }
}