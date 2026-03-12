using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public static class SaveSystem
{
    private static string DefaultPath =>
        Path.Combine(Application.persistentDataPath, "jennifer_save.json");

    public static void Save(SaveData data) => SaveToPath(data, DefaultPath);
    public static SaveData Load() => LoadFromPath(DefaultPath);

    public static void SaveToPath(SaveData data, string path)
    {
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(path, json);
    }

    public static SaveData LoadFromPath(string path)
    {
        if (!File.Exists(path)) return new SaveData();
        try
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<SaveData>(json) ?? new SaveData();
        }
        catch { return new SaveData(); }
    }
}
