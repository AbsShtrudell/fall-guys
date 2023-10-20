using RootMotion.Dynamics;
using System;
using System.Collections;
using UnityEngine;

namespace FallGuys
{
    public class CharacterController : MonoBehaviour
    {
        [SerializeField] private float _deathDuration;

        [Header("References")]
        [SerializeField] private PuppetMaster _puppetMaster;
        [SerializeField] private Transform _focusPoint;

        public Transform FocusPoint { get { return _focusPoint; } }

        private Coroutine _killCoroutine;

        public event Action<CharacterController> OnCharacterKilled;

        public void Kill()
        {
            if (_killCoroutine != null)
                StopCoroutine(_killCoroutine);

            _killCoroutine = StartCoroutine(KillCharacterCoroutine());
        }

        private IEnumerator KillCharacterCoroutine()
        {
            _puppetMaster.Freeze();
            yield return new WaitForSeconds(_deathDuration);
            OnCharacterKilled?.Invoke(this);

        }
    }
}