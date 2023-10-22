using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace FallGuys 
{
    public class HealthBarController : MonoBehaviour
    {
        [Zenject.Inject] private PlayerController _playerController;

        [SerializeField] private Slider _healthSlider;
        [SerializeField] private Gradient _gradient;
        [SerializeField] private RectTransform _iconRect;
        [SerializeField] private Image _sliderImage;
        [SerializeField] private Image _iconImage;

        private void OnEnable()
        {
            _playerController.ActiveCharChanged += FocusCharacter;

            if (_playerController.ActiveCharacter != null)
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

        private void UpdateHealthBar(float health)
        {
            _iconRect.DOShakeScale(0.5f, 0.5f, 10);
            float h = health;
            _sliderImage.color = _gradient.Evaluate(h);
            _iconImage.color = _gradient.Evaluate(h);
            _healthSlider.value = h;
        }
    }
}