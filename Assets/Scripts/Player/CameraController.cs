using UnityEngine;

namespace FallGuys
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CharacterController _target;

        [Header("Position")]
        [SerializeField] private bool _smoothFollow;
        [SerializeField] private Vector3 _offset = new Vector3(0, 1.5f, 0.5f);
        [SerializeField] private float _followSpeed = 2f;

        [Header("Rotation")]
        [SerializeField] private float _rotationSensitivity = 3.5f;
        [SerializeField] private float _yMinLimit = -20;
        [SerializeField] private float _yMaxLimit = 80;

        [Header("Distance")]
        [SerializeField] private float _distance = 10.0f;

        private float _currentX;
        private float _currentY;

        private Vector3 _position;
        private Quaternion _rotation = Quaternion.identity;
        private Vector3 _smoothPosition;

        public CharacterController Target 
        { 
            get { return _target; } 
            set { if (value != null) _target = value; } 
        }

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        protected virtual void FixedUpdate()
        {
            UpdateTransform();
        }

        protected virtual void LateUpdate()
        {
            UpdateInput();
        }

        public void UpdateInput()
        {
            // delta rotation
            _currentX += Input.GetAxis("Mouse X") * _rotationSensitivity;
            _currentY = ClampAngle(_currentY - Input.GetAxis("Mouse Y") * _rotationSensitivity, _yMinLimit, _yMaxLimit);
        }


        // Clamping Euler angles
        private float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360) angle += 360;
            if (angle > 360) angle -= 360;
            return Mathf.Clamp(angle, min, max);
        }

        public void UpdateTransform()
        {
            UpdateTransform(Time.deltaTime);
        }

        public void UpdateTransform(float deltaTime)
        {
            // Rotation
            _rotation = Quaternion.AngleAxis(_currentX, Vector3.up) * Quaternion.AngleAxis(_currentY, Vector3.right);

            if (_target != null)
            {
                // Smooth follow
                if (!_smoothFollow) _smoothPosition = _target.FocusPoint.position;
                else _smoothPosition = Vector3.Lerp(_smoothPosition, _target.FocusPoint.position, deltaTime * _followSpeed);

                // Position
                Vector3 t = _smoothPosition + _rotation * _offset;
                Vector3 f = _rotation * -Vector3.forward;

                _position = t + f * _distance;

                // Translating the camera
                transform.position = _position;
            }

            transform.rotation = _rotation;
        }
    }
}