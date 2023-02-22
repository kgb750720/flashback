using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SaveSystem))]
[RequireComponent(typeof(ObjectPool))]
[RequireComponent(typeof(StaticData))]
[RequireComponent(typeof(MusicManager))]
[RequireComponent(typeof(EventCenter))]
[RequireComponent(typeof(ScenesManager))]
[RequireComponent(typeof(AssetsManager))]
[RequireComponent(typeof(UIManager))]
[RequireComponent(typeof(InputManager))]
[RequireComponent(typeof(TimeManager))]
[RequireComponent(typeof(CameraManager))]

public class Game : SingletonMono<Game>
{
    //全局访问功能
    [HideInInspector]
    public SaveSystem SaveSystem;
    [HideInInspector]
    public ObjectPool ObjectPool;
    [HideInInspector]
    public StaticData StaticData;
    [HideInInspector]
    public AssetsManager AssetsManager;
    [HideInInspector] 
    public UIManager UIManager;
    [HideInInspector] 
    public EventCenter EventCenter;
    [HideInInspector] 
    public InputManager InputManager;
    [HideInInspector] 
    public MusicManager MusicManager;
    [HideInInspector]
    public ScenesManager ScenesManager;
    [HideInInspector]
    public TimeManager TimeManager;
    [HideInInspector]
    public CameraManager CameraManager;

    public GameObject player;
    public PlayerManager PlayerManager;

    public bool isGameOver = false;

    //游戏入口
    protected void Start()
    {
        Debug.Log("Game Start!");
        
        //确保Game对象存在
        Object.DontDestroyOnLoad(this.gameObject);
        
        //全局单例赋值
        SaveSystem = SaveSystem.Instance;
        ObjectPool = ObjectPool.Instance;
        StaticData = StaticData.Instance;
        AssetsManager = AssetsManager.Instance;
        UIManager = UIManager.Instance;
        EventCenter = EventCenter.Instance;
        InputManager = InputManager.Instance;
        MusicManager = MusicManager.Instance;
        ScenesManager = ScenesManager.Instance;
        TimeManager = TimeManager.Instance;
        CameraManager = CameraManager.Instance;
        
        //全局设置
        Application.targetFrameRate = 120;

        //进入开始界面
        Instance.ScenesManager.LoadSceneAsyn(Consts.StartMenu, () =>
        {
            Game.Instance.UIManager.currentLayer = EUILayer.Back;
            StartMenuArgs e = new StartMenuArgs();
            Game.Instance.UIManager.ShowPanel<UIStart>(Consts.UIStart, e, EUILayer.Back);
            player = Game.Instance.ObjectPool.GetObject(Consts.A_Player + "Player");
            PlayerManager = player.GetComponent<PlayerManager>();
            player.SetActive(false);
        });
    }
}
