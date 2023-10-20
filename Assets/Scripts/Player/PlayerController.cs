using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FallGuys
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private CharacterController _activeCharacter;
        [SerializeField] private CharacterController _secondCharacter;
        [SerializeField] private CameraController _camera;

        public CharacterController ActiveCharacter { get { return _activeCharacter; } }

        private void OnEnable()
        {
            _activeCharacter.OnCharacterKilled += ChangeCharacter;
        }

        private void OnDisable()
        {
            _activeCharacter.OnCharacterKilled -= ChangeCharacter;
        }

        private void ChangeCharacter(CharacterController character)
        {
            _camera.Target = _secondCharacter;
        }
    }
}
