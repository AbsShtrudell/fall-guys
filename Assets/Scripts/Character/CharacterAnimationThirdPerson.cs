using RootMotion.Demos;
using UnityEngine;

namespace FallGuys
{
    public class CharacterAnimationThirdPerson: CharacterAnimationBase {

        [SerializeField] private CharacterThirdPerson _characterController;
		[SerializeField] private float _turnSensitivity = 0.2f; // Animator turning sensitivity
		[SerializeField] private float _turnSpeed = 5f; // Animator turning interpolation speed
		[SerializeField] private float _runCycleLegOffset = 0.2f; // The offset of leg positions in the running cycle
		[Range(0.1f,3f)] [SerializeField] float _animSpeedMultiplier = 1; // How much the animation of the character will be multiplied by
		
		protected Animator _animator;
		private Vector3 _lastForward;
		private const string _groundedDirectional = "Grounded Directional", _groundedStrafe = "Grounded Strafe";
		private float _deltaAngle;
        private bool _lastJump;

        protected override void Start() {
			base.Start();

			_animator = GetComponent<Animator>();

			_lastForward = transform.forward;
		}
		
		public override Vector3 GetPivotPoint() {
			return _animator.pivotPosition;
		}
		
		// Is the Animator playing the grounded animations?
		public override bool animationGrounded {
			get {
				return _animator.GetCurrentAnimatorStateInfo(0).IsName(_groundedDirectional) || _animator.GetCurrentAnimatorStateInfo(0).IsName(_groundedStrafe);
			}
		}
		
		// Update the Animator with the current state of the character controller
		protected virtual void Update() {
			if (Time.deltaTime == 0f) return;

            animatePhysics = _animator.updateMode == AnimatorUpdateMode.AnimatePhysics;

			// Jumping
			if (_characterController.animState.jump) {
                if (!_lastJump)
                {
                    float runCycle = Mathf.Repeat(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime + _runCycleLegOffset, 1);
                    float jumpLeg = (runCycle < 0.5f ? 1 : -1) * _characterController.animState.moveDirection.z;
                    
                    _animator.SetFloat("JumpLeg", jumpLeg);
                }
			}
            _lastJump = _characterController.animState.jump;
			
			// Calculate the angular delta in character rotation
			float angle = -GetAngleFromForward(_lastForward) - _deltaAngle;
			_deltaAngle = 0f;
			_lastForward = transform.forward;
			angle *= _turnSensitivity * 0.01f;
			angle = Mathf.Clamp(angle / Time.deltaTime, -1f, 1f);
			
			// Update Animator params
			_animator.SetFloat("Turn", Mathf.Lerp(_animator.GetFloat("Turn"), angle, Time.deltaTime * _turnSpeed));
			_animator.SetFloat("Forward", _characterController.animState.moveDirection.z);
			_animator.SetFloat("Right", _characterController.animState.moveDirection.x);
			_animator.SetBool("OnGround", _characterController.animState.onGround);
			
			if (!_characterController.animState.onGround) {
				_animator.SetFloat ("Jump", _characterController.animState.yVelocity);
			}
			
			// the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector
			if (_characterController.animState.onGround && _characterController.animState.moveDirection.z > 0f) {
				_animator.speed = _animSpeedMultiplier;
			} else {
				// but we don't want to use that while airborne
				_animator.speed = 1;
			}
		}

		// Call OnAnimatorMove manually on the character controller because it doesn't have the Animator component
		void OnAnimatorMove() {
			// For not using root rotation in Turn value calculation 
			Vector3 f = _animator.deltaRotation * Vector3.forward;
			_deltaAngle += Mathf.Atan2(f.x, f.z) * Mathf.Rad2Deg;

            if (_characterController.FullRootMotion)
            {
                _characterController.transform.position += _animator.deltaPosition;
                _characterController.transform.rotation *= _animator.deltaRotation;
            }
            else
            {
                _characterController.Move(_animator.deltaPosition, _animator.deltaRotation);
            }
		}
	}
}
