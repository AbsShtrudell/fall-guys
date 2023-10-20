using FallGuys;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FallGuys
{
    public class GameModeController : MonoBehaviour
    {
        [Zenject.Inject]
        private EndZone _endZone;
        [Zenject.Inject]
        private StartLine _startLine;
        [Zenject.Inject]
        private Lifes _lifes;
        [Zenject.Inject]
        private PlayerController _playerController;
        public event Action<float> Won;
        public event Action<float> Loose;

        private float startTime = -1;

        private void OnEnable()
        {
            _endZone.ReachedEnd += OnReached;
            _lifes.LifesChanged += OnLifesChanged;
            _startLine.Crossed += OnCrossed;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnCrossed()
        {
            if(startTime < 0)
            {
                startTime = Time.time;
            }
        }

        private void OnLifesChanged(int obj)
        {
            if (obj <= 0)
            {
                ShowCursor();
                Loose?.Invoke(1);
                Debug.Log("Loose");
            }
        }

        private void OnReached()
        {
            ShowCursor();
            Won?.Invoke(Time.time - startTime);
            Debug.Log("Won");
        }

        private void ShowCursor()
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }

        public void Restart()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex);
        }

    }
}