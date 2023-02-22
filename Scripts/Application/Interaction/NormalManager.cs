using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractiveSystem
{
    public class NormalManager : Singleton<NormalManager>
    {
        [HideInInspector] public Transform lastInteraction;
        [HideInInspector] public Transform lastRevive;
    }
}
