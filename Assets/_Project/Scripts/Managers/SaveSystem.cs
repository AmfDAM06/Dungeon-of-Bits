using UnityEngine;
using System.IO;

[System.Serializable]
public class SaveData
{
    public int floor;
    public int currentHealth;
    public int maxHealth;

    // --- NUEVO: Añadimos la mochila a la partida guardada ---
    public int healthPotions;
    public int invisPotions;
    public int strengthPotions;
    public int bombs;
}

public static class SaveSystem
{
    private static string path = Application.persistentDataPath + "/savegame.json";

    public static void Save(int floor, int currentHealth, int maxHealth)
    {
        SaveData data = new SaveData
        {
            floor = floor,
            currentHealth = currentHealth,
            maxHealth = maxHealth,

            // --- NUEVO: Extraemos los datos del inventario al guardar ---
            healthPotions = PlayerInventory.globalHealthPotions,
            invisPotions = PlayerInventory.globalInvisPotions,
            strengthPotions = PlayerInventory.globalStrengthPotions,
            bombs = PlayerInventory.globalBombs
        };
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(path, json);
        Debug.Log("Partida Guardada en: " + path);
    }

    public static SaveData Load()
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<SaveData>(json);
        }
        return null;
    }

    public static bool HasSaveFile() { return File.Exists(path); }
    public static void DeleteSave() { if (File.Exists(path)) File.Delete(path); }
}