using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveable
{
    void SaveableRegister();

    //存
    void SaveData();

    //取
    void LoadData();
}
