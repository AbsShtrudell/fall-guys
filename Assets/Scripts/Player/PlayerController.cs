using RootMotion.Demos;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace FallGuys
{
    public class PlayerController : MonoBehaviour
    {
        [Zenject.Inject] private Spawner _spawner;
        [Zenject.Inject] private Lifes _lifes;
        [SerializeField] private float _deathDuration = 1;
        [SerializeField] private CharacterController _activeCharacter;

        private Coroutine _respawnCoroutine;

        public CharacterController ActiveCharacter { get { return _activeCharacter; } }

        public event Action<CharacterController> ActiveCharChanged;

        private void OnEnable()
        {
            ChangeCharacter(_spawner.Spawn());

        }

        private void OnKilled(CharacterController character)
        {
            Respawn();
        }

        private void ChangeCharacter(CharacterController character)
        {
            if (_activeCharacter != null)
            {
                _activeCharacter.OnCharacterKilled -= OnKilled;
            }
            _activeCharacter = character;
            ActiveCharChanged?.Invoke(_activeCharacter);

            _activeCharacter.OnCharacterKilled += OnKilled;
        }

        private void Respawn()
        {
            if (_respawnCoroutine != null)
                StopCoroutine(_respawnCoroutine);

            _respawnCoroutine = StartCoroutine(RespawnCoroutine());
        }

        private IEnumerator RespawnCoroutine()
        {
            var ch = _activeCharacter;
            _activeCharacter = null;

            yield return new WaitForSeconds(_deathDuration);

            _lifes.Die();

            Destroy(ch.gameObject);

            if (_lifes.CurrentLifes > 0)
                ChangeCharacter(_spawner.Spawn());
        }
    }
}
