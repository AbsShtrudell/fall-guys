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
        [SerializeField] private Sprite _looseIcon;

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
            _messageText.text = "������";
            _endIcon.sprite = _winIcon;
            _timeText.text = "�����: " + time.ToString();
        }

        private void OnLoose(float time)
        {
            _resultPanel.SetActive(true);
            _messageText.text = "���������";
            _endIcon.sprite = _looseIcon;
            _timeText.text = "�����: " + time.ToString();
        }

        public void RestartAction()
        {
            _gameMode.Restart();
        }
    }
}