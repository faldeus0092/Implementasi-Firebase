using Firebase.Analytics;

// Anaytics Manager
public static class AnalyticsManager
{
    private static void LogEvent (string eventName, params Parameter[] parameters)
    {
        // method untuk menembakkan firebase
        FirebaseAnalytics.LogEvent(eventName, parameters);
    }

    // untuk unlock upgrade
    public static void LogUpgradeEvent(int resourceIndex, int level)
    {
        // memakai event dan parameter yang tersedia di firebase, agar dapat muncul sebagai report data di analytics firebase
        LogEvent(
            FirebaseAnalytics.EventLevelUp,
            new Parameter(FirebaseAnalytics.ParameterIndex, resourceIndex.ToString()),
            new Parameter(FirebaseAnalytics.ParameterLevel, level)
            );
        // resourceIndex digunakan sebagai ID, jadi sebaiknya menggunakan string bukan integer
    }

    // untuk unlock achievement
    public static void LogUnlockEvent(int resourceIndex)
    {
        LogEvent(
            FirebaseAnalytics.EventUnlockAchievement,
            new Parameter(FirebaseAnalytics.ParameterIndex, resourceIndex.ToString())
            );
    }

    public static void SetUserProperties(string name, string value)
    {
        FirebaseAnalytics.SetUserProperty(name, value);
    }
}