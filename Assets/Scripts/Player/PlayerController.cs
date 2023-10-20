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

        [SerializeField] private CharacterController _activeCharacter;

        public CharacterController ActiveCharacter { get { return _activeCharacter; } }

        public event Action<CharacterController> ActiveCharChanged;

        private void OnEnable()
        {
            ChangeCharacter(_spawner.Spawn());

        }

        private void OnKilled(CharacterController character)
        {
            _lifes.Die();
            if (_lifes.CurrentLifes > 0)
                ChangeCharacter(_spawner.Spawn());
        }

        private void ChangeCharacter(CharacterController character)
        {
            if(_activeCharacter != null)
                _activeCharacter.OnCharacterKilled -= OnKilled;

            _activeCharacter = character;
            ActiveCharChanged?.Invoke(_activeCharacter);

            _activeCharacter.OnCharacterKilled += OnKilled;
        }
    }
}
