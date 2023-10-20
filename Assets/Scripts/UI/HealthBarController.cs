using System;
using UnityEngine;
using UnityEngine.UI;

namespace FallGuys 
{
    public class HealthBarController : MonoBehaviour
    {
        [Zenject.Inject] private PlayerController _playerController;

        [SerializeField] private Slider healthSlider;

        private void OnEnable()
        {
            _playerController.ActiveCharChanged += FocusCharacter;
            FocusCharacter(_playerController.ActiveCharacter);
        }

        private void OnDisable()
        {
            _playerController.ActiveCharChanged -= FocusCharacter;
        }

        private void FocusCharacter(CharacterController controller)
        {
            controller.Health.HealthChange += UpdateHealthBar;
            UpdateHealthBar(controller.Health.CurrentHealth);
        }

        private void UpdateHealthBar(int health)
        {
            healthSlider.value = (float)health / (float)_playerController.ActiveCharacter.Health.MaxHealth;
        }
    }
}