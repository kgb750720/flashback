using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractiveSystem
{
    public class MedicalPro : InteractionManager
    {
        public int healthUp = 1;

        protected override void Trigger(Collider2D _collider2D)
        {
            if (_collider2D.tag == "Player")
            {
                PlayerManager player = _collider2D.GetComponentInParent<PlayerManager>();

                if (player != null)
                    HealthMaxUp(player);
            }

            Destroy(this);
        }

        private void HealthMaxUp(PlayerManager _player)
        {
            _player.playerMaxHp += healthUp;
        }
    }
}