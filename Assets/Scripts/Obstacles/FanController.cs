using System.Collections;
using UnityEngine;

namespace FallGuys
{
    public class FanController : MonoBehaviour
    {
        [SerializeField] private float _changeInterval = 10f;

        [Header("Components")]
        [SerializeField] private WindVolume _windVolume;
        [SerializeField] private ParticleSystem _particles;
        [SerializeField] private Animator _animator;

        private Coroutine _changeDirectionCoroutine;
        private void Start()
        {
            StartWindChangeRoutine();
        }

        private void StartWindChangeRoutine()
        {
            if (_changeDirectionCoroutine != null)
            {
                StopCoroutine(_changeDirectionCoroutine);
            }
            _changeDirectionCoroutine = StartCoroutine(ChangeWindDirection());
        }

        private IEnumerator ChangeWindDirection()
        {
            while (true)
            {
                yield return new WaitForSeconds(_changeInterval);

                _animator.SetBool("Backwards", !_animator.GetBool("Backwards"));

            }
        }
    }
}
