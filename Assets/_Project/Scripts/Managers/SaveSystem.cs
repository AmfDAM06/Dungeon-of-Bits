using UnityEngine;
using System.IO; // Necesario para leer y escribir archivos en el ordenador

// 1. CREAMOS UNA CAJA PARA LOS DATOS
// Esta clase contiene exactamente lo que queremos recordar al cerrar el juego
[System.Serializable]
public class SaveData
{
    public int floor;
    public int currentHealth;
    public int maxHealth;
}

// 2. EL MOTOR DE GUARDADO
public static class SaveSystem
{
    // Ruta segura donde Unity guarda las partidas en tu ordenador
    private static string path = Application.persistentDataPath + "/savegame.json";

    public static void Save(int floor, int currentHealth, int maxHealth)
    {
        SaveData data = new SaveData { floor = floor, currentHealth = currentHealth, maxHealth = maxHealth };
        string json = JsonUtility.ToJson(data); // Convierte los datos a texto
        File.WriteAllText(path, json); // Lo guarda en un archivo .json
        Debug.Log("Partida Guardada en: " + path);
    }

    public static SaveData Load()
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<SaveData>(json); // Convierte el texto de vuelta a datos
        }
        return null;
    }

    public static bool HasSaveFile()
    {
        return File.Exists(path);
    }

    public static void DeleteSave()
    {
        if (File.Exists(path)) File.Delete(path);
    }
}