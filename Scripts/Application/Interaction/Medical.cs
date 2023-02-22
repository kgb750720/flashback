using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractiveSystem
{
    public class Medical : InteractionManager
    {
        public int recovery = 1;

        protected override void Trigger(Collider2D _collider2D)
        {
            if (_collider2D.tag == "Player")
            {
                PlayerManager player = _collider2D.GetComponentInParent<PlayerManager>();

                if (player != null)
                    Recovery(player);
            }
        }

        private void Recovery(PlayerManager _player)
        {
            if (_player.PlayerHp + recovery <= _player.playerMaxHp)
            {
                MusicManager.Instance.PlaySound("HpRecover",null,false);
                _player.PlayerHp += recovery;
                Destroy(gameObject);
            }
            else
            {
                _player.PlayerHp = _player.playerMaxHp;
            }
        }
    }
}
