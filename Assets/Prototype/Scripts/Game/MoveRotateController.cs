using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game{

    public class MoveRotateController : MonoBehaviour
    {

        [Header("Setting")]
		[Tooltip("Move speed of the character in m/s")]
		public float MoveSpeed = 2.0f;
		[Tooltip("How fast the character turns to face movement direction")]
		[Range(0.0f, 0.3f)]
		public float RotationSmoothTime = 0.12f;
		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 10.0f;
		[Tooltip("The height the player can jump")]
		public float JumpHeight = 1.2f;
		[Tooltip("How long can jump command to")]
		public float JumpBufferTime = 0.50f;
		[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
		public float FallTimeout = 0.15f;
		public float GravityScale = 1.0f;
		[Header("Player Grounded")]
		public bool Grounded = true;
		[Tooltip("Useful for rough ground")]
		public float GroundedOffset = -0.14f;
		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.28f;
		[Tooltip("What layers the character uses as ground")]
		public LayerMask GroundLayers;

		[Header("Cinemachine")]
		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 70.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -30.0f;

		[SerializeField] GameObject Gun;
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		[SerializeField] GameObject CinemachineCameraTarget;
		[Header("Listening Channel")]
		[SerializeField] GameObjectEventChannelSO LocalCharacterReadyEvent;

		// local character
		private bool _isLocalCharacterReady = false;

        // cinemachine
		private float _cinemachineTargetYaw;
		private float _cinemachineTargetPitch;

        // timeout deltatime
		private float _fallTimeoutDelta;

        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private GameObject _localCharacter;
        private Rigidbody _rigidbody;

		private void OnEnable() {
			Debug.Log("Register character ready event");
			LocalCharacterReadyEvent.OnEventRaised += OnLocalCharacterReady;
		}

		private void OnDisable() {
			LocalCharacterReadyEvent.OnEventRaised -= OnLocalCharacterReady;
		}

        private void OnLocalCharacterReady(GameObject localCharacter)
        {
			Debug.Log("handler character ready controller");
            _localCharacter = localCharacter;
			_rigidbody = _localCharacter.GetComponent<Rigidbody>();

			// reset our timeouts on start
			_fallTimeoutDelta = FallTimeout;

			// set ready
			_isLocalCharacterReady = true;

			// set up reference
			CharacterBehaviour charBehave = _localCharacter.GetComponent<CharacterBehaviour>();
			CinemachineCameraTarget = charBehave.CameraFollowTarget;
			Gun = charBehave.Gun;
        }

        private void Awake() {
            // get a reference to our main camera
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}
        }

        private void FixedUpdate() {
			if(_isLocalCharacterReady == false) return;
            GroundedCheck();
			ApplyGravity();
        }

        private void ApplyGravity()
        {
            _rigidbody.AddForce(Vector3.down * 9.8f * GravityScale, ForceMode.Acceleration);
        }

        private void GroundedCheck()
		{
			// set sphere position, with offset
			Vector3 spherePosition = new Vector3(_localCharacter.transform.position.x, _localCharacter.transform.position.y - GroundedOffset, _localCharacter.transform.position.z);
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
		}


		private float _speed;
		private float _targetRotation = 0.0f;
		private float _rotationVelocity;
		public void WalkingMove(Vector2 move)
		{
			// get moving direction and face it
			// normalise input direction
			Vector3 inputDirection = new Vector3(move.x, 0.0f, move.y).normalized;

			// if there is a move input rotate player when the player is moving
			if (move != Vector2.zero)
			{
				_targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
				float rotation = Mathf.SmoothDampAngle(_localCharacter.transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

				// rotate to face input direction relative to camera position
				_localCharacter.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
			}

			// set target speed based on move speed, sprint speed and if sprint is pressed
			float targetSpeed = MoveSpeed;
			Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

			// if there is no inpu t, set the target speed to 0
			if (move == Vector2.zero) targetSpeed = 0.0f;

			// a reference to the players current horizontal velocity
			Vector3 currentHorizontalVelocity = new Vector3(_rigidbody.velocity.x, 0.0f, _rigidbody.velocity.z);

			float speedOffset = MoveSpeed / 10.0f;
			float acceleration = SpeedChangeRate;
			Vector3 targetVelocity = targetDirection * targetSpeed;
			Vector3 velocityDiff = targetVelocity - currentHorizontalVelocity;
			// accelerate or decelerate to target velocity
			if (velocityDiff.magnitude > speedOffset)
			{
				// accelerate 
				_rigidbody.AddForce(velocityDiff.normalized * acceleration, ForceMode.Acceleration);
			}

		}

        public void CameraRotation(Vector2 look)
		{
			// if there is an input and camera position is not fixed
			if (look.sqrMagnitude >= _threshold)
			{
				_cinemachineTargetYaw += look.x * Time.deltaTime;
				_cinemachineTargetPitch += look.y * Time.deltaTime;
			}

			// clamp our rotations so our values are limited 360 degrees
			_cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
			_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

			// Cinemachine will follow this target
			CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0.0f);

			// handler Gun Pitch
			// Gun.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch, 0, 0);
		}

		private void Update() {
			if(Grounded){ 
				// reset the fall timeout timer
				_fallTimeoutDelta = FallTimeout;
			}
			else{

				if (_fallTimeoutDelta >= 0.0f){
					_fallTimeoutDelta -= Time.deltaTime;
				}
			}

			if(_buffJump){
				if(_buffJumpDetla >= 0.0f){
					_buffJumpDetla -= Time.deltaTime;
					if(Grounded){
						_buffJump = false;
						Jump();
					}
				}
				else{
					_buffJump  = false;
				}
			}
		}

		private bool _buffJump = false;
		private float _buffJumpDetla = 0.0f;
		public void Jump()
		{
			
			if (Grounded)
			{
				// Jump
				// F = M * V
				// V = sqrt(2 * g * h)
				float forceMagnitube = _rigidbody.mass * (Mathf.Sqrt(2 *( 9.8f * GravityScale) * JumpHeight ));
				_rigidbody.AddForce(Vector3.up * forceMagnitube, ForceMode.Impulse);
			}
			else{
				_buffJump = true;
				_buffJumpDetla = JumpBufferTime;
			}
		}
        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

        private void OnDrawGizmosSelected() {
			if(_localCharacter == null) return;
			Vector3 spherePosition = new Vector3(_localCharacter.transform.position.x, _localCharacter.transform.position.y - GroundedOffset, _localCharacter.transform.position.z);
            Gizmos.color = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Gizmos.DrawSphere(spherePosition, GroundedRadius);
        }

    }

}
