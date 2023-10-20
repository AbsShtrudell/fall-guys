using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FallGuys
{
    public class Lifes : MonoBehaviour
    {
        [SerializeField] private int _lifes = 3;

        private int _currentLifes = 3;

        public int CurrentLifes { get => _currentLifes; private set => _currentLifes = value; }

        public event Action<int> LifesChanged;

        private void Awake()
        {
            CurrentLifes = _lifes;
        }

        public void Die()
        {
            CurrentLifes--;
            LifesChanged?.Invoke(CurrentLifes);
        }
    }
}