using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LasterSave : MonoBehaviour, ISaveable
{
    private void Start()
    {
        SaveableRegister();
    }

    public void LoadData() { }

    public void SaveableRegister()
    {
        Game.Instance.SaveSystem.RegisterSaveable(this);
    }

    public void SaveData()
    {
        if (SceneManager.GetActiveScene().name == "3.Level_201")
            SaveSystem.Instance.currentSave.laster102 = true;
        if (SceneManager.GetActiveScene().name == "3.Level_205")
            SaveSystem.Instance.currentSave.laster204 = true;
    }

    public void OnDestroy()
    {
        Game.Instance.SaveSystem.RemoveSaveable(this);
    }
}
