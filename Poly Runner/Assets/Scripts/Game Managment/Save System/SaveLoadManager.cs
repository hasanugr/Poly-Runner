using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;

public static class SaveLoadManager
{
    public static string directory = "SaveData";
    public static string fileName = "dadt.sav";

    public static void Save(PlayerData pd)
    {
        if (!DirectoryExist())
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/" + directory);
        } 

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(GetFullPath());
        bf.Serialize(file, pd);
        file.Close();
    }

    public static PlayerData Load()
    {
        if (SaveExist())
        {
            // Get saved game data and send it to current data.
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(GetFullPath(), FileMode.Open);
            PlayerData pd = bf.Deserialize(file) as PlayerData;
            file.Close();

            return pd;
        }else
        {
            // If there is no saved data, create defaulth data and send it to current data.
            PlayerData pd = new PlayerData();
            pd.AddDefaultValues();

            return pd;
        }
    }

    private static bool SaveExist()
    {
        return File.Exists(GetFullPath());
    }

    private static bool DirectoryExist()
    {
        return Directory.Exists(Application.persistentDataPath + "/" + directory);
    }

    private static string GetFullPath()
    {
        return Application.persistentDataPath + "/" + directory + "/" + fileName;
    }
}
