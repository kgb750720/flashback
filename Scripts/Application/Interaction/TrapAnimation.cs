using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractiveSystem
{
    public class TrapAnimation : MonoBehaviour
    {
        public List<Sprite> sprites = new List<Sprite>();
        public float time = 0.04F;
        public bool isLoop;

        private float delTime;
        private bool isPlay;
        private int index = 0;

        private void Awake()
        {
            delTime = time;

            isPlay = true;
        }

        private void ChangeSprite(bool _isLoop)
        {
            time -= Time.deltaTime;
            if (time < 0)
            {
                GetComponent<SpriteRenderer>().sprite = sprites[index];
                time = delTime;
                index++;

                if (index >= sprites.Count)
                {
                    if (isLoop) index = 0;
                    else
                    {
                        index = sprites.Count - 1;
                        isPlay = false;
                    }
                }
            }    
        }

        private void Update()
        {
            if (isPlay) ChangeSprite(isLoop);
        }

        private void OnDisable()
        {
            index = 0;
            GetComponent<SpriteRenderer>().sprite = sprites[0];
        }
    }
}
