using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FallGuys
{
    [RequireComponent(typeof(Collider))]
    public class DamageDealler : MonoBehaviour
    {
        [SerializeField] private int _damage = 10;

        public int Damage { get { return _damage; } }

        private void OnCollisionEnter(Collision collision)
        {
            collision.rigidbody.TryGetComponent<DamageZone>(out var zone);

            if (zone != null)
                zone.Health.DealDamage(_damage);
        }
    }
}