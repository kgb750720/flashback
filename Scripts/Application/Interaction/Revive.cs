using UnityEngine;
using EffectSystem;

namespace InteractiveSystem
{
    public class Rivive : InteractionManager
    {
        public string activeEffectName = "GroundYellow";
        public string alwaysEffectName = "GroundBlue";

        private Effect active;
        private Effect always; 

        private void Start()
        {
            always = EffectManager.Instance.AddEffect2D(alwaysEffectName,transform.position,gameObject);
            always.RecycleEffect(5F, true);
        }

        protected override void Trigger(Collider2D collision)
        {
            base.Trigger(collision);

            if (collision.tag == "Player")
            {
                active = EffectManager.Instance.AddEffect2D(activeEffectName, transform.position, gameObject);
                normal.lastRevive = transform;

                PlayerManager player = collision.GetComponentInParent<PlayerManager>();

                if (player != null)
                    Recovery(player);
            }
        }

        protected override void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                ObjectPool.Instance.RecycleObject(active.gameObject);
            }
        }

        private void Recovery(PlayerManager _player)
        {
            _player.playerHp = _player.playerMaxHp;
        }
    }
}
