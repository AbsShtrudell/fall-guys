using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FallGuys
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private int _maxHealth = 100;
        [SerializeField] private float _damageDelay = 1f;

        public int MaxHealth { get { return _maxHealth; } }
        public int CurrentHealth { get; private set; }

        public event Action<int> HealthChange;

        private float _lastHit;

        private void OnEnable()
        {
            CurrentHealth = MaxHealth;
        }

        public void DealDamage(int damage)
        {
            if (Time.time < _lastHit + _damageDelay) return;

            _lastHit = Time.time;
            CurrentHealth -= damage;
            Debug.Log(CurrentHealth.ToString());
            HealthChange?.Invoke(CurrentHealth);
        }
    }
}