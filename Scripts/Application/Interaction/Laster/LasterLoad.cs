using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LasterLoad : MonoBehaviour, ISaveable
{
    private bool check102;
    private bool check204;

    private void Start()
    {
        SaveableRegister();
    }

    public void SaveableRegister()
    {
        Game.Instance.SaveSystem.RegisterSaveable(this);
    }

    public void SaveData() { }

    public void LoadData()
    {
        if (SceneManager.GetActiveScene().name == "3.Level_102")
            check102 = SaveSystem.Instance.currentSave.laster102;
        if (SceneManager.GetActiveScene().name == "3.Level_204")
            check204 = SaveSystem.Instance.currentSave.laster204;

        if (SceneManager.GetActiveScene().name == "3.Level_102" && check102 == true)
            Destroy(gameObject);
        if (SceneManager.GetActiveScene().name == "3.Level_204" && check204 == true)
            Destroy(gameObject);

        Debug.LogError("¶ÁÈ¡Êý¾Ý " + check102 + "\n" + check204);
    }

    public void OnDestroy()
    {
        Game.Instance.SaveSystem.RemoveSaveable(this);
    }
}
