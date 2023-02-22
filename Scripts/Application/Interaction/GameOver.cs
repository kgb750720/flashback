using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;

namespace InteractiveSystem
{
    public class GameOver : MonoBehaviour
    {
        //对话的名字
        public string dialogName_trueEnd = Consts.dia_5;
        public string dialogName_falseEnd = Consts.dia_6;
        public float waitTime = 2F;  //从boss死亡到启动对话的时间

        private DialogCompleteArgs dca = new DialogCompleteArgs();
        private UnityAction<DialogCompleteArgs> actionOver;
        private UnityAction action;
        private float timeToOver;
        private bool trueEnd;
        private Game game;
        private VideoPlayer videoPlayer;
        private bool isLoadVideo = false;

        public string DialogNameFalseEnd
        {
            get => dialogName_falseEnd;
            set => dialogName_falseEnd = value;
        }

        private void Awake()
        {
            game = Game.Instance;
            action = EndToOver;
            actionOver = PlayMovie;
            game.EventCenter.AddEventListener(Consts.B_BossDeath, action);
            game.EventCenter.AddEventListener(Consts.E_TalkCompleted, actionOver);
            videoPlayer = this.transform.GetComponent<VideoPlayer>();
            videoPlayer.Stop();
            videoPlayer.playOnAwake = false;
            videoPlayer.isLooping = false;
            videoPlayer.Prepare();
            
            videoPlayer.prepareCompleted += (source) =>
            {
                isLoadVideo = true;
            };
            videoPlayer.loopPointReached += (source) =>
            {
                Game.Instance.UIManager.HidePanel(Consts.UIPause);
                Game.Instance.UIManager.HidePanel(Consts.UIBattle);
                Game.Instance.UIManager.vidoePlayer.SetActive(false);
                Game.Instance.isGameOver = false;
                if (EffectSystem.EffectManager.Instance.effectPool.Count != 0)
                {
                    foreach (var values in EffectSystem.EffectManager.Instance.effectPool.Values)
                    {
                        for (int i = 0; i < values.Count; ++i)
                        {
                            ObjectPool.Instance.RecycleObject(values[i].gameObject);
                        }
                    }
                }

                Game.Instance.player.SetActive(false);
                Game.Instance.ScenesManager.LoadSceneAsyn(Consts.StartMenu, () =>
                {
                    Game.Instance.UIManager.ShowPanel<UIStart>(Consts.UIStart, null, EUILayer.Back);
                });

            };
        }

        private void EndToOver()
        {
            StartCoroutine(GetEnd());
        }

        private void PlayMovie(DialogCompleteArgs dca)
        {
            Debug.LogError("开始播放");

            if (dca.dialogName == Consts.dia_5 || dca.dialogName == Consts.dia_6)
            {
                if (isLoadVideo)
                {
                    Game.Instance.isGameOver = true;
                    Game.Instance.MusicManager.StopBgMusic();
                    Game.Instance.UIManager.vidoePlayer.SetActive(true);
                    videoPlayer.Play();
                }
                    
            }
        }

        public void prepared()
        {
            Debug.LogError("准备完毕");
        }

        private IEnumerator GetEnd()
        {
            yield return new WaitForSeconds(waitTime);

            int end = game.SaveSystem.currentSave.getedGoods;
            int count = StaticData.Instance.itemDatas.Count;
            int trueInt = (int)(Mathf.Pow(2, count) - 1);

            if (end == trueInt) trueEnd = true;
            else trueEnd = false;

            yield return DialogStartShow();
        }

        private IEnumerator DialogStartShow()
        {
            //StartCoroutine(GetEnd());

            Game.Instance.InputManager.canControl = false;
            yield return Game.Instance.CameraManager.FadeIn(1, Color.black);

            DialogArgs e = new DialogArgs();
            e.dialogName = trueEnd == true ? dialogName_trueEnd : dialogName_falseEnd;
            Game.Instance.UIManager.ShowPanel<DialogueSystemPanel>(Consts.DialogueSystemPanel, e, EUILayer.Dialog);
        }

        private void OnDestroy()
        {
            action = EndToOver;
            actionOver = PlayMovie;
            game.EventCenter.RemoveEventListener(Consts.B_BossDeath, action);
            game.EventCenter.RemoveEventListener(Consts.E_TalkCompleted, actionOver);
        }
    }
}
