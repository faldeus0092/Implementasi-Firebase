using UnityEngine;
using Firebase.Storage;
using System.Collections;
using System.Text;
using System.Threading.Tasks;

// mengatur userprogress data untuk disimpan atau diload
public static class UserDataManager
{
    private const string PROGRESS_KEY = "Progress";
    public static UserProgressData Progress = new UserProgressData();

    public static void LoadFromLocal()
    {
        // cek apakah ada data yang tersimpan
        if (!PlayerPrefs.HasKey(PROGRESS_KEY))
        {
            // JIKA TIDAK ADA BUAT BARU dan upload ke cloud
            // Progress = new UserProgressData();
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

        // jika load from local
        if (uploadToCloud)
        {
            AnalyticsManager.SetUserProperties("gold", Progress.Gold.ToString());
            byte[] data = Encoding.Default.GetBytes(json);
            StorageReference targetStorage = GetTargetCloudStorage();

            targetStorage.PutBytesAsync(data);
        }
    }

    public static bool HasResources(int index)
    {
        return index + 1 <= Progress.ResourcesLevels.Count;
    }

    public static IEnumerator LoadFromCloud(System.Action onComplete)
    {
        StorageReference targetStorage = GetTargetCloudStorage();

        bool isCompleted = false;
        bool isSuccessfull = false;
        const long maxAllowedSize = 1024 * 1024;
        targetStorage.GetBytesAsync(maxAllowedSize).ContinueWith((Task<byte[]> task) =>
        {
            if (!task.IsFaulted)
            {
                string json = Encoding.Default.GetString(task.Result);
                Progress = JsonUtility.FromJson<UserProgressData>(json);
                isSuccessfull = true;
            }
            isCompleted = true;
        });

        while (!isCompleted)
        {
            yield return null;
        }

        // simpan data ketika sukses mendownload
        if (isSuccessfull)
        {
            Save();
        }
        else
        {
            // load dari local
            LoadFromLocal();
        }

        onComplete?.Invoke();
    }

    private static StorageReference GetTargetCloudStorage()
    {
        // device ID digunakan sebagai nama file di cloud
        string deviceID = SystemInfo.deviceUniqueIdentifier;
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
        return storage.GetReferenceFromUrl($"{storage.RootReference}/{deviceID}");
    }

}
