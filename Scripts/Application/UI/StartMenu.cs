using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartMenuArgs e = new StartMenuArgs();
        Game.Instance.UIManager.ShowPanel<UIStart>(Consts.UIStart, e, EUILayer.Back);
    }
    
}
