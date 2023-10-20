using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FallGuys
{
    public class DamageZone : MonoBehaviour
    {
        [SerializeField] private Health _health;

        public Health Health { get { return _health; } }
    }
}