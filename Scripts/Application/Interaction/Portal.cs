using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using EffectSystem;
using UnityEngine.Events;

namespace InteractiveSystem
{
    public class Portal : DialogueObject
    {
        //[CustomLabel("门的动画时间")]
        public float AnimationTime = 1F;
        //[CustomLabel("动画播放完的等待时间")]
        public float WaitTime = 1F;
        //[CustomLabel("想要到达的场景的名字")]
        public string targetSceneName;
        //[CustomLabel("想要到达的门的名字")]
        public string targetDoorName;
        //[CustomLabel("添加的描述信息")]
        public string content = "[F]键进行交互";
        //[CustomLabel("动画Shader所在的SpriteRenderer")]
        public SpriteRenderer doorSpriteRenderer;

        //[CustomLabel("特效的偏移量X(黄色特效)")]
        public float OffestEffectX = 0F;
        //[CustomLabel("特效的偏移量Y(黄色特效)")]
        public float OffestEffectY = -1.5F;

        private float start = 0;
        private bool isOpenning = false;
        private bool isInteractive = false;
        private Effect active;
        private GameObject character;
        private SaveDataArgs pta = new SaveDataArgs();

        protected override void Awake()
        {
            base.Awake();

            pta.sceneName = targetSceneName;
            pta.doorName = targetDoorName;
        }

        protected override void Trigger(Collider2D collision)
        {
            base.Trigger(collision);

            if (collision.tag == "Player")
            {
                isInteractive = true;
                character = collision.gameObject;
                active = EffectManager.Instance.AddWorldEffect2D("GroundYellow", transform.position);
                active.RecycleEffect(5, true);

                active.transform.position = new Vector3(active.transform.position.x + OffestEffectX
                    ,active.transform.position.y + OffestEffectY ,0);

                Game.Instance.EventCenter.EventTrigger(Consts.T_D_Start);
            }
        }

        protected override void OnTriggerExit2D(Collider2D collision)
        {
            base.OnTriggerExit2D(collision);

            if (collision.tag == "Player")
            {
                isInteractive = false;
                if (active != null) ObjectPool.Instance.RecycleObject(active.gameObject);
                
                Game.Instance.EventCenter.EventTrigger(Consts.T_D_Over);
            }
        }

        protected override void Apply()
        {
            if (!isInteractive) return;
            //执行传送逻辑/场景加载
            Game.Instance.InputManager.canControl = false;
            isInteractive = false;
            isOpenning = true;
        }

        public void SetSpriteRendererFill(GameObject obj, float fillAmount, float fillType = 1, float fillFlip = 1)
        {
            if (doorSpriteRenderer == null) return;

            Material mat = doorSpriteRenderer.material;

            fillType = Mathf.Clamp01(fillType);
            fillAmount = Mathf.Clamp01(fillAmount);
            mat.SetFloat("_FillType", fillType);
            mat.SetFloat("_FillAmount", fillAmount);
            mat.SetFloat("_FillFlip", fillFlip);
        }

        private IEnumerator WaitForAnimation()
        {
            yield return new WaitForSeconds(WaitTime);

            //转换场景时需要检查并回收所有缓存池对象
            if (EffectManager.Instance.effectPool.Count != 0)
            {
                foreach (var values in EffectManager.Instance.effectPool.Values)
                {
                    for (int i = 0; i < values.Count; ++i)
                    {
                        ObjectPool.Instance.RecycleObject(values[i].gameObject);
                    }
                }
            }
            
            Debug.Log("Show Interactive!");

            Game.Instance.ScenesManager.LoadSceneAsyn(targetSceneName, () =>
            {
                FindObjectOverScene();
                Game.Instance.UIManager.currentLayer = EUILayer.Battle;
                Game.Instance.InputManager.inputState = InputState.Gaming;
                Game.Instance.EventCenter.EventTrigger(Consts.S_SAVE, pta);
                Game.Instance.SaveSystem.Load();
                Game.Instance.InputManager.canControl = true;
            });
        }

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
                    Debug.LogError(string.Format("场景{0}中找不到名为{1}的物体", targetSceneName, gameObjects[i].name));
                }
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (active != null) ObjectPool.Instance.RecycleObject(active.gameObject);
            Game.Instance.EventCenter.EventTrigger(Consts.T_D_Over);
        }

        protected override void Update()
        {
            base.Update();

            if (isOpenning)
            {
                start += Time.deltaTime;

                float temp = Mathf.Lerp(0, 1, start / AnimationTime);
                SetSpriteRendererFill(gameObject, temp);

                if (temp >= 1)
                {
                    isOpenning = false;
                    StartCoroutine(WaitForAnimation());
                }
            }
        }

        protected override void SetContent()
        {
            Tip.SetText(content);
        }
    }
}
