using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace InteractiveSystem
{
    public class TriggerNPC : NPC
    {
        //[CustomLabel("��Ҫ����ĳ���������")]
        public string targetSceneName;
        //[CustomLabel("��Ҫ������ŵ�����")]
        public string targetDoorName;

        private GameObject character;
        private UnityAction<DialogCompleteArgs> action;
        private SaveDataArgs pta = new SaveDataArgs();
        private DialogCompleteArgs dca = new DialogCompleteArgs();

        protected override void Awake()
        {
            base.Awake();

            dca.dialogName = dialogName;
            pta.sceneName = targetSceneName;
            pta.doorName = targetDoorName;

            action = ToBoss;
            Game.Instance.EventCenter.AddEventListener(Consts.E_TalkCompleted, action);
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            base.OnTriggerEnter2D(collision);
            base.Apply();

            if (collision.tag == "Player")
            {                                                                                                                                                                                                                                               
                character = collision.gameObject;
            }
        }

        protected override void AndroidOver() { }
        protected override void AndroidStart() { }

        private void FindObjectOverScene()
        {
            Scene gameScene = SceneManager.GetSceneByName(targetSceneName);
            GameObject[] gameObjects = gameScene.GetRootGameObjects();

            for (int i = 0; i < gameObjects.Length; ++i)
            {
                if (gameObjects[i].name == targetDoorName)
                {
                    character.transform.position = gameObjects[i].transform.position;
                    pta.rivivePos = gameObjects[i].transform.position;
                    Debug.Log("rivivePos:" + pta.rivivePos);
                    break;
                }
                if (i == gameObjects.Length - 1 && gameObjects[i].name != targetDoorName)
                {
                    Debug.LogError(string.Format("����{0}���Ҳ�����Ϊ{1}������", targetSceneName, gameObjects[i].name));
                }
            }
        }

        protected override IEnumerator DialogStartShow()
        {
            Game.Instance.InputManager.canControl = false;
            yield return Game.Instance.CameraManager.FadeIn(1, Color.black);

            DialogArgs e = new DialogArgs();
            e.dialogName = dialogName;
            Game.Instance.UIManager.ShowPanel<DialogueSystemPanel>(Consts.DialogueSystemPanel, e, EUILayer.Dialog , null , false);
        }

        private void ToBoss(DialogCompleteArgs dca)
        {
            if (dca.dialogName == dialogName)
            {
                Game.Instance.ScenesManager.LoadSceneAsyn(targetSceneName, () =>
                {
                    FindObjectOverScene();
                    Game.Instance.UIManager.currentLayer = EUILayer.Battle;
                    Game.Instance.InputManager.inputState = InputState.Gaming;
                    Game.Instance.EventCenter.EventTrigger(Consts.S_SAVE, pta);
                    Game.Instance.SaveSystem.Load();
                });
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Game.Instance.EventCenter.RemoveEventListener(Consts.E_TalkCompleted, action);
        }
    }
}
