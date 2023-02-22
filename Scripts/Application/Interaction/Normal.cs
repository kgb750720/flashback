using UnityEngine;

namespace InteractiveSystem
{
    public class Normal : InteractionManager
    {
        protected override void Trigger(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                normal.lastInteraction = transform;
            }
        }
    }
}
