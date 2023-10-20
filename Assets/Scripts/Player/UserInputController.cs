using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FallGuys
{
    public class UserInputController : MonoBehaviour
    {
        [Zenject.Inject] 
        private PlayerController _playerController;
        [Zenject.Inject] 
        private CameraController _cameraController;

        public struct State
        {
            public Vector3 move;
            public Vector3 lookPos;
        }

        private CharacterInputs _characterActions;
        private InputAction _movementAction;

        public State state = new State();

        public event Action Jump;

        private void Awake()
        {
            _characterActions = new CharacterInputs();
        }

        private void OnEnable()
        {
            _movementAction = _characterActions.Default.Movement;

            _characterActions.Default.Jump.performed += OnJump;

            _characterActions.Enable();
        }

        private void OnDisable()
        {
            _characterActions.Default.Jump.performed += OnJump;

            _characterActions.Disable();
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            Jump?.Invoke();
        }

        private void HandleMovement()
        {
            if (_playerController.ActiveCharacter == null) return;

            float h = _movementAction.ReadValue<Vector2>().x;
            float v = _movementAction.ReadValue<Vector2>().y;

            // calculate move direction
            Vector3 move = _cameraController.transform.rotation * new Vector3(h, 0f, v).normalized;

            // Flatten move vector to the character.up plane
            if (move != Vector3.zero)
            {
                Vector3 normal = _playerController.ActiveCharacter.transform.up;
                Vector3.OrthoNormalize(ref normal, ref move);
                state.move = move;
            }
            else state.move = Vector3.zero;

            // calculate the head look target position
            state.lookPos = _playerController.ActiveCharacter.transform.position + _cameraController.transform.forward * 100f;
        }

        protected virtual void Update()
        {
            HandleMovement();
        }

    }
}
