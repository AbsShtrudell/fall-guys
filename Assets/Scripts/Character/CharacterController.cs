using RootMotion.Dynamics;
using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace FallGuys
{
    public class CharacterController : MonoBehaviour
    {
        [SerializeField] private float _deathDuration;
        [SerializeField] private float _deathY = -10;

        [Header("References")]
        [SerializeField] private PuppetMaster _puppetMaster;
        [SerializeField] private Transform _focusPoint;

        private bool _dead = false;

        public Health Health { get; private set; }

        public Transform FocusPoint { get { return _focusPoint; } }

        public event Action<CharacterController> OnCharacterKilled;

        private void Awake()
        {
            Health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            Health.HealthChange += OnHealthChange;
            _puppetMaster.Resurrect();
        }

        private void OnDisable()
        {
            Health.HealthChange -= OnHealthChange;
        }

        private void Update()
        {
            if (_focusPoint.position.y < _deathY && !_dead)
                Kill();
        }

        private void OnHealthChange(float health)
        {
            if (health <= 0 && !_dead)
                Kill();
        }

        public void Kill()
        {
            _dead = true;
            _puppetMaster.Kill();
            OnCharacterKilled?.Invoke(this);
        }

        public class Factory : IFactory<CharacterController>
        {
            DiContainer _container;
            CharacterController _characterController;

            public Factory(CharacterController character, DiContainer container) 
            {
                _container = container;
                _characterController = character;
            }

            public CharacterController Create()
            {
                CharacterController character;

                if (_container != null)
                {
                    character = _container.InstantiatePrefabForComponent<CharacterController>(_characterController);
                }
                else
                {
                    character = Instantiate(_characterController.gameObject).GetComponent<CharacterController>();
                }

                return character;
            }
        }
    }
}