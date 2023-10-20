using UnityEngine;
using System.Collections;
using RootMotion.Demos;
using RootMotion;

namespace FallGuys {
	public class CharacterThirdPerson : CharacterBase {


		// Animation state
		public struct AnimState {
			public Vector3 moveDirection; // the forward speed
			public bool jump; // should the character be jumping?
			public bool onGround; // is the character grounded
			public float yVelocity; // y velocity of the character
		}

        [Zenject.Inject] protected UserInputController _userControl;

        [Header("References")]
        [SerializeField] protected CharacterAnimationBase _characterAnimation; // the animation controller

		[Header("Movement")]
        [SerializeField] protected bool _smoothPhysics = true; // If true, will use interpolation to smooth out the fixed time step.
        [SerializeField] protected float _smoothAccelerationTime = 0.2f; // The smooth acceleration of the speed of the character (using Vector3.SmoothDamp)
        [SerializeField] protected float _linearAccelerationSpeed = 3f; // The linear acceleration of the speed of the character (using Vector3.MoveTowards)
        [SerializeField] protected float _platformFriction = 7f;                   // the acceleration of adapting the velocities of moving platforms
        [SerializeField] protected float _groundStickyEffect = 4f;             // power of 'stick to ground' effect - prevents bumping down slopes.
        [SerializeField] protected float _maxVerticalVelocityOnGround = 3f;        // the maximum y velocity while the character is grounded
        [SerializeField] protected float _velocityToGroundTangentWeight = 0f;	// the weight of rotating character velocity vector to the ground tangent

		[Header("Rotation")]
        [SerializeField] protected bool _lookInCameraDirection; // should the character be looking in the same direction that the camera is facing
        [SerializeField] protected float _turnSpeed = 5f;                  // additional turn speed added when the player is moving (added to animation root rotation)
        [SerializeField] protected float _stationaryTurnSpeedMlp = 1f;           // additional turn speed added when the player is stationary (added to animation root rotation)

        [Header("Jumping and Falling")]
        [SerializeField] protected bool _smoothJump = true; // If true, adds jump force over a few fixed time steps, not in a single step
        [SerializeField] protected float _airSpeed = 6f; // determines the max speed of the character while airborne
        [SerializeField] protected float _airControl = 2f; // determines the response speed of controlling the character while airborne
        [SerializeField] protected float _jumpPower = 12f; // determines the jump force applied when jumping (and therefore the jump height)
        [SerializeField] protected float _jumpRepeatDelayTime = 0f;            // amount of time that must elapse between landing and being able to jump again
        [SerializeField] protected bool _doubleJumpEnabled;
        [SerializeField] protected float _doubleJumpPowerMlp = 1f;

        /// <summary>
        /// Enable this while playing an animation that should be driven 100% by root motion, such as climbing walls
        /// </summary>
        public bool FullRootMotion { get; set; }

        public bool OnGround { get; private set; }
		public AnimState animState = new AnimState();

		protected Vector3 _moveDirection;
		private Animator _animator;
		private Vector3 _normal, _platformVelocity, _platformAngularVelocity;
		private float _jumpEndTime, _forwardMlp, _groundDistance, _lastAirTime, _stickyForce;
		private Vector3 _moveDirectionVelocity;
        private float _fixedDeltaTime;
		private Vector3 _fixedDeltaPosition;
		private Quaternion _fixedDeltaRotation = Quaternion.identity;
		private Vector3 _gravity;
		private float _velocityY;

		// Use this for initialization
		protected override void Start () {
			base.Start();

			_animator = GetComponent<Animator>();
			if (_animator == null) 
				_animator = _characterAnimation.GetComponent<Animator>();

			OnGround = true;
			animState.onGround = true;
		}

        private void OnEnable()
        {
			_userControl.Jump += OnJump;
        }

        private void OnDisable()
        {
            _userControl.Jump -= OnJump;
        }

        void OnAnimatorMove() {
			Move(_animator.deltaPosition, _animator.deltaRotation);
		}

		// When the Animator moves
		public override void Move(Vector3 deltaPosition, Quaternion deltaRotation) {
            // Accumulate delta position, update in FixedUpdate to maintain consitency
            _fixedDeltaTime += Time.deltaTime;
			_fixedDeltaPosition += deltaPosition;
			_fixedDeltaRotation *= deltaRotation;
		}

        void FixedUpdate() {
            _gravity = FullRootMotion? Vector3.zero: GetGravity();

			Vector3 verticalVelocity = V3Tools.ExtractVertical(r.velocity, _gravity, 1f);
			_velocityY = verticalVelocity.magnitude;
			if (Vector3.Dot(verticalVelocity, _gravity) > 0f) _velocityY = -_velocityY;

			// Smoothing out the fixed time step
			r.interpolation = _smoothPhysics? RigidbodyInterpolation.Interpolate: RigidbodyInterpolation.None;
			_characterAnimation.smoothFollow = _smoothPhysics;

            // Move
			MoveFixed(_fixedDeltaPosition);

            _fixedDeltaTime = 0f;
			_fixedDeltaPosition = Vector3.zero;

			r.MoveRotation(transform.rotation * _fixedDeltaRotation);
			_fixedDeltaRotation = Quaternion.identity;

			Rotate();

			GroundCheck (); // detect and stick to ground

			// Friction
			if (_userControl.state.move == Vector3.zero && _groundDistance < airborneThreshold * 0.5f) HighFriction();
			else ZeroFriction();

			bool stopSlide = !FullRootMotion && OnGround && _userControl.state.move == Vector3.zero && r.velocity.magnitude < 0.5f && _groundDistance < airborneThreshold * 0.5f;

			if (stopSlide) {
				r.useGravity = false;
				r.velocity = Vector3.zero;
			} else if (gravityTarget == null) r.useGravity = true;
	
        }

        protected virtual void Update() {
            // Fill in animState
			animState.onGround = OnGround;
			animState.moveDirection = GetMoveDirection();
			animState.yVelocity = Mathf.Lerp(animState.yVelocity, _velocityY, Time.deltaTime * 10f);
		}


		private void MoveFixed(Vector3 deltaPosition) {

            Vector3 velocity = _fixedDeltaTime > 0f? deltaPosition / _fixedDeltaTime: Vector3.zero;

            // Add velocity of the rigidbody the character is standing on
            if (!FullRootMotion)
            {
                velocity += V3Tools.ExtractHorizontal(_platformVelocity, _gravity, 1f);

                if (OnGround)
                {
                    // Rotate velocity to ground tangent
                    if (_velocityToGroundTangentWeight > 0f)
                    {
                        Quaternion rotation = Quaternion.FromToRotation(transform.up, _normal);
                        velocity = Quaternion.Lerp(Quaternion.identity, rotation, _velocityToGroundTangentWeight) * velocity;
                    }
                }
                else
                {
                    // Air move
                    //Vector3 airMove = new Vector3 (userControl.state.move.x * airSpeed, 0f, userControl.state.move.z * airSpeed);
                    Vector3 airMove = V3Tools.ExtractHorizontal(_userControl.state.move * _airSpeed, _gravity, 1f);
                    velocity = Vector3.Lerp(r.velocity, airMove, Time.deltaTime * _airControl);
                }

                if (OnGround && Time.time > _jumpEndTime)
                {
                    r.velocity = r.velocity - transform.up * _stickyForce * Time.deltaTime;
                }

                // Vertical velocity
                Vector3 verticalVelocity = V3Tools.ExtractVertical(r.velocity, _gravity, 1f);
                Vector3 horizontalVelocity = V3Tools.ExtractHorizontal(velocity, _gravity, 1f);

                if (OnGround)
                {
                    if (Vector3.Dot(verticalVelocity, _gravity) < 0f)
                    {
                        verticalVelocity = Vector3.ClampMagnitude(verticalVelocity, _maxVerticalVelocityOnGround);
                    }
                }

                r.velocity = horizontalVelocity + verticalVelocity;
            } else
            {
                r.velocity = velocity;
            }

            // Dampering forward speed on the slopes (Not working since Unity 2017.2)
            //float slopeDamper = !onGround? 1f: GetSlopeDamper(-deltaPosition / Time.deltaTime, normal);
            //forwardMlp = Mathf.Lerp(forwardMlp, slopeDamper, Time.deltaTime * 5f);
            _forwardMlp = 1f;
		}

		// Get the move direction of the character relative to the character rotation
		private Vector3 GetMoveDirection() {

			_moveDirection = Vector3.SmoothDamp(_moveDirection, new Vector3(0f, 0f, _userControl.state.move.magnitude), ref _moveDirectionVelocity, _smoothAccelerationTime);
			_moveDirection = Vector3.MoveTowards(_moveDirection, new Vector3(0f, 0f, _userControl.state.move.magnitude), Time.deltaTime * _linearAccelerationSpeed);
			return _moveDirection * _forwardMlp;
		}

		// Rotate the character
		protected virtual void Rotate() {
			if (gravityTarget != null) r.MoveRotation (Quaternion.FromToRotation(transform.up, transform.position - gravityTarget.position) * transform.rotation);
			if (_platformAngularVelocity != Vector3.zero) r.MoveRotation (Quaternion.Euler(_platformAngularVelocity) * transform.rotation);

			float angle = GetAngleFromForward(GetForwardDirection());
			
			if (_userControl.state.move == Vector3.zero) angle *= (1.01f - (Mathf.Abs(angle) / 180f)) * _stationaryTurnSpeedMlp;

			// Rotating the character
			//RigidbodyRotateAround(characterAnimation.GetPivotPoint(), transform.up, angle * Time.deltaTime * turnSpeed);
			r.MoveRotation(Quaternion.AngleAxis(angle * Time.deltaTime * _turnSpeed, transform.up) * r.rotation);
		}

		// Which way to look at?
		private Vector3 GetForwardDirection() {
			bool isMoving = _userControl.state.move != Vector3.zero;

			if (isMoving) return _userControl.state.move;
			return _lookInCameraDirection? _userControl.state.lookPos - r.position: transform.forward;
		}

        protected void OnJump()
		{
			animState.jump = Jump();
        }

		protected virtual bool Jump() {
			if (!_characterAnimation.animationGrounded) return false;
			if (Time.time < _lastAirTime + _jumpRepeatDelayTime) return false;

			// Jump
			OnGround = false;
			_jumpEndTime = Time.time + 0.1f;

            Vector3 jumpVelocity = _userControl.state.move * _airSpeed;
            jumpVelocity += transform.up * _jumpPower;

            if (_smoothJump)
            {
                StopAllCoroutines();
                StartCoroutine(JumpSmooth(jumpVelocity - r.velocity));
            } else
            {
                r.velocity = jumpVelocity;
            }

            return true;
		}

        // Add jump velocity smoothly to avoid puppets launching to space when unpinned during jump acceleration
        private IEnumerator JumpSmooth(Vector3 jumpVelocity)
        {
            int steps = 0;
            int stepsToTake = 3;
            while (steps < stepsToTake)
            {
                r.AddForce(jumpVelocity / stepsToTake, ForceMode.VelocityChange);
                steps++;
                yield return new WaitForFixedUpdate();
                animState.jump = false;
            }
        }

		// Is the character grounded?
		private void GroundCheck () {
			Vector3 platformVelocityTarget = Vector3.zero;
			_platformAngularVelocity = Vector3.zero;
			float stickyForceTarget = 0f;

            // Spherecasting
            RaycastHit hit = GetSpherecastHit();

			//normal = hit.normal;
			_normal = transform.up;
			//groundDistance = r.position.y - hit.point.y;
			_groundDistance = Vector3.Project(r.position - hit.point, transform.up).magnitude;

			// if not jumping...
			bool findGround = Time.time > _jumpEndTime && _velocityY < _jumpPower * 0.5f;

			if (findGround) {
				bool g = OnGround;
				OnGround = false;

				// The distance of considering the character grounded
				float groundHeight = !g? airborneThreshold * 0.5f: airborneThreshold;

				//Vector3 horizontalVelocity = r.velocity;
				Vector3 horizontalVelocity = V3Tools.ExtractHorizontal(r.velocity, _gravity, 1f);

				float velocityF = horizontalVelocity.magnitude;

				if (_groundDistance < groundHeight) {
					// Force the character on the ground
					stickyForceTarget = _groundStickyEffect * velocityF * groundHeight;

					// On moving platforms
					if (hit.rigidbody != null) {
						platformVelocityTarget = hit.rigidbody.GetPointVelocity(hit.point);
						_platformAngularVelocity = Vector3.Project(hit.rigidbody.angularVelocity, transform.up);
					}

					// Flag the character grounded
					OnGround = true;
				}
			}

			// Interpolate the additive velocity of the platform the character might be standing on
			_platformVelocity = Vector3.Lerp(_platformVelocity, platformVelocityTarget, Time.deltaTime * _platformFriction);
            if (FullRootMotion) _stickyForce = 0f;

            _stickyForce = stickyForceTarget;//Mathf.Lerp(stickyForce, stickyForceTarget, Time.deltaTime * 5f);

			// remember when we were last in air, for jump delay
			if (!OnGround) _lastAirTime = Time.time;
		}
	}
}
