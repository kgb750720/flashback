using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class SaveSystem : SingletonMono<SaveSystem>
{
    public SaveData currentSave;
    public string currentSaveDir;

    private string jsonFolder;

    public List<ISaveable> saveableList;

    private void Start()
    {
        jsonFolder = Application.persistentDataPath + "/Save/";
        Debug.Log(jsonFolder);
        currentSave = new SaveData();
        saveableList = new List<ISaveable>();
        Game.Instance.EventCenter.AddEventListener(Consts.E_SaveData, ResponseSaveData);
        Game.Instance.EventCenter.AddEventListener(Consts.E_LoadData, ResponseLoadData);
    }

    private void OnDestroy()
    {
        Game.Instance.EventCenter.RemoveEventListener(Consts.E_SaveData, ResponseSaveData);
        Game.Instance.EventCenter.RemoveEventListener(Consts.E_LoadData, ResponseLoadData);
    }
    

    #region Json

    public void SaveByJson(string saveFileName, object data)
    {
        var json = JsonUtility.ToJson(data);

        try
        {
            File.WriteAllText(saveFileName, json);
        }
        catch (System.Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public T LoadFromJson<T>(string saveFileName)
    {
        try
        {
            var json = File.ReadAllText(saveFileName);
            var data = JsonUtility.FromJson<T>(json);

            return data;
        }
        catch (System.Exception e)
        {
            Console.WriteLine(e);
            return default;
        }
    }

    public void DeleteSaveFile(string saveFileName)
    {
        try
        {
            File.Delete(saveFileName);
        }
        catch (System.Exception e)
        {
            Console.WriteLine(e);
        }
    }

    #endregion


    #region NewtonJson

    public void RegisterSaveable(ISaveable saveable)
    {
        if(!saveableList.Contains(saveable)) saveableList.Add(saveable);
    }

    public void RemoveSaveable(ISaveable saveable)
    {
        if (saveableList.Contains(saveable)) saveableList.Remove(saveable);
    }

    public void ResponseSaveData()
    {
        Debug.Log("Save Data!");
        if (File.Exists(currentSaveDir))
        {
            File.Delete(currentSaveDir);
        }
        Save();
    }
    
    public void ResponseLoadData()
    {
        if (!Load())
        {
            Debug.LogError("Load Data Failed!");
        }
    }

    public void Save()
    {
        var resultPath = jsonFolder + currentSaveDir;
        Debug.Log("SaveDir" + resultPath);

        foreach (var saveable in saveableList)
        {
            saveable.SaveData();
        }
        
        var jsonData = JsonConvert.SerializeObject(currentSave, Formatting.Indented);
        if (!File.Exists(resultPath))
        {
            Directory.CreateDirectory(jsonFolder);
        }
        File.WriteAllText(resultPath, jsonData);
    }

    public bool Load()
    {
        var resultPath = jsonFolder + currentSaveDir;
        
        if (File.Exists(resultPath))
        {
            var stringData = File.ReadAllText(resultPath);

            currentSave = JsonConvert.DeserializeObject<SaveData>(stringData);
        }
        else
        {
            currentSave = Game.Instance.StaticData.initialData;
        }

        foreach (var saveable in saveableList)
        {
            saveable.LoadData();
        }
        
        return true;
    }

    public void SceneInit()
    {
        foreach (var saveable in saveableList)
        {
            saveable.LoadData();
        }
    }
    
    #endregion
}
