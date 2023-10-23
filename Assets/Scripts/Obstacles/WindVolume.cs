using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FallGuys
{
    public class WindVolume : MonoBehaviour
    {
        [SerializeField] private Transform _windSource;
        [SerializeField] private Vector3 _windDirection = Vector3.forward;
        [SerializeField] private float _windStrength = 10f;
        [SerializeField] private float _changeInterval = 10f;

        private List<Rigidbody> _rigidbodiesInVolume = new List<Rigidbody>();
        private Coroutine _windChangeCoroutine;

        private Vector3 _currentWindDirection;

        private void Start()
        {
            _currentWindDirection = _windDirection;
            //StartWindChangeRoutine();
        }

        private void StartWindChangeRoutine()
        {
            if (_windChangeCoroutine != null)
            {
                StopCoroutine(_windChangeCoroutine);
            }
            _windChangeCoroutine = StartCoroutine(ChangeWindDirection());
        }

        private IEnumerator ChangeWindDirection()
        {
            while (true)
            {
                yield return new WaitForSeconds(_changeInterval);
                _currentWindDirection = _currentWindDirection * -1;
            }
        }

        public void ReverseWind()
        {
            _currentWindDirection = _currentWindDirection * -1;
        }

        private void OnTriggerEnter(Collider other)
        {
            Rigidbody r = other.GetComponent<Rigidbody>();
            if (r != null)
            {
                _rigidbodiesInVolume.Add(r);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Rigidbody r = other.GetComponent<Rigidbody>();
            if (r != null)
            {
                _rigidbodiesInVolume.Remove(other.GetComponent<Rigidbody>());
            }
        }

        private void FixedUpdate()
        {
            foreach (var r in _rigidbodiesInVolume)
            {
                float distance = Vector3.Distance(_windSource.position, r.transform.position);
                float windForce = _windStrength / distance;

                r.AddForce(_currentWindDirection.normalized * windForce, ForceMode.Force);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(new Ray(transform.position, transform.position + _windDirection));
        }
    }
}