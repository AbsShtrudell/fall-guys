using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FallGuys {
    public class EndMessageController : MonoBehaviour
    {
        [Zenject.Inject]
        private GameModeController _gameMode;

        [SerializeField] private Sprite _winIcon;
        [SerializeField] private Color _winColor;
        [SerializeField] private Sprite _looseIcon;
        [SerializeField] private Color _looseColor;

        [Header("References")]
        [SerializeField] private TMPro.TMP_Text _messageText;
        [SerializeField] private TMPro.TMP_Text _timeText;
        [SerializeField] private Image _endIcon;
        [SerializeField] private GameObject _resultPanel;

        private void Awake()
        {
            _resultPanel.SetActive(false);
        }

        private void OnEnable()
        {
            _gameMode.Won += OnWin;
            _gameMode.Loose += OnLoose;
        }

        private void OnWin(float time)
        {
            _resultPanel.SetActive(true);

            _messageText.text = "Победа";

            _endIcon.sprite = _winIcon;
            _endIcon.color = _winColor;

            _timeText.text = "Время: " + FormatTime(time);
        }

        private void OnLoose(float time)
        {
            _resultPanel.SetActive(true);

            _messageText.text = "Поражение";

            _endIcon.sprite = _looseIcon;
            _endIcon.color = _looseColor;

            _timeText.enabled = false;
        }

        public void RestartAction()
        {
            _gameMode.Restart();
        }

        public string FormatTime(float seconds)
        {
            int minutes = (int)seconds / 60;
            int remainingSeconds = (int)seconds % 60;

            string formattedTime = string.Format("{0:00}:{1:00}", minutes, remainingSeconds);

            return formattedTime;
        }
    }
}