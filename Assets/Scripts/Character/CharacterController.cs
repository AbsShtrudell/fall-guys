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

        [Header("References")]
        [SerializeField] private PuppetMaster _puppetMaster;
        [SerializeField] private Transform _focusPoint;

        public Health Health { get; private set; }

        public Transform FocusPoint { get { return _focusPoint; } }

        private Coroutine _killCoroutine;

        public event Action<CharacterController> OnCharacterKilled;

        private void Awake()
        {
            Health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            Health.HealthChange += OnHealthChange;
        }

        private void OnDisable()
        {
            Health.HealthChange -= OnHealthChange;
        }

        private void OnHealthChange(int health)
        {
            if (health <= 0)
                Kill();
        }

        public void Kill()
        {
            if (_killCoroutine != null)
                StopCoroutine(_killCoroutine);

            _killCoroutine = StartCoroutine(KillCharacterCoroutine());
        }

        private IEnumerator KillCharacterCoroutine()
        {
            _puppetMaster.Kill();
            yield return new WaitForSeconds(_deathDuration);
            OnCharacterKilled?.Invoke(this);
            GameObject.Destroy(gameObject);
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