using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovingIKScript : MonoBehaviour
{
	public static MovingIKScript _instance;

    private CharacterInput _input;

    private Vector2 _armMovement;

	bool leftArmGrip = false;
	bool rightArmGrip = false;

	[Header("Bones")]
	[SerializeField]
	private Transform _hipBone;
	[SerializeField]
	Transform _leftArmTarget;
	[SerializeField]
	Transform _rightArmTarget;

	[SerializeField]
	Transform _leftArmStick;
	[SerializeField]
	Transform _rightArmStick;

	[SerializeField]
	Transform _leftArmEndBone;
	[SerializeField]
	Transform _rightArmEndBone;

	[SerializeField]
	Transform _armatureRoot;

	[SerializeField]
	Transform _leftStartingPos;
	[SerializeField]
	Transform _rightStartingPos;

	[SerializeField]
	Transform _headBone;


	[SerializeField]
	float _armMovementFactor = 5;
	[SerializeField]
	float _maxExtension = 1;


	Vector3 _startingLeftPosition;
	Vector3 _startingRightPosition;

	public float armMoveDuration = 1.5f;
	public float timeElapsed =0;




	//EVENTS DE MOVEMENT
	public delegate void OnBodyMoving(Vector3 direction, float distance);
	public static OnBodyMoving onBodyMoving;

	bool _bodyMoved = false;

	private void Awake()
    {
		//SINGLETON
		if (_instance != null && _instance != this)
			Destroy(gameObject);    

		_instance = this;

		_input = new CharacterInput();

        _input.Player.GripLeftArm.started += i => OnGripLeft();
        _input.Player.GripLeftArm.canceled += i => OnUngripLeft();
        _input.Player.GripRightArm.started += i => OnGripRight();
        _input.Player.GripRightArm.canceled += i => OnUngripRight();
        _input.Player.MoveArm.performed += i => _armMovement = i.ReadValue<Vector2>();
        _input.Player.MoveArm.canceled += i => _armMovement = Vector2.zero;
	}

	private void Start()
	{
		_startingLeftPosition = _leftArmEndBone.position;
		_startingRightPosition = _rightArmEndBone.position;
	}
	private void OnEnable()
	{
		_input.Player.Enable();
	}

	private void Update()
	{
		//Move the StickTarget
		if (!leftArmGrip && rightArmGrip)
		{
			_leftArmStick.position = new Vector3(_leftStartingPos.position.x + _armMovement.x * _armMovementFactor, _leftStartingPos.position.y + _armMovement.y * _armMovementFactor, _leftArmTarget.position.z);


		}
		else if (leftArmGrip && !rightArmGrip)
		{
			_rightArmStick.position = new Vector3(_rightStartingPos.position.x + _armMovement.x * _armMovementFactor, _rightStartingPos.position.y + _armMovement.y * _armMovementFactor, _leftArmTarget.position.z);
		}

		//Move the torso when out of reach
		if (leftArmGrip && !rightArmGrip)
		{
			float distanceToHandle = Vector3.Distance(_leftArmEndBone.position, _leftArmTarget.position);
			Vector3 directionToHandle = (_leftArmTarget.position - _leftArmEndBone.position).normalized;
			directionToHandle.z = 0;
			if (distanceToHandle >= .1f)
			{
				_armatureRoot.position += directionToHandle * 5f * Time.deltaTime;
				if (!_bodyMoved)
				{
					onBodyMoving?.Invoke(directionToHandle, distanceToHandle);
					_bodyMoved = true;
				}

			}
			_headBone.up = _rightArmTarget.position + new Vector3(0, .5f, 0) - _headBone.position;
		}
		if (rightArmGrip && !leftArmGrip)
		{
			float distanceToHandle = Vector3.Distance(_rightArmEndBone.position, _rightArmTarget.position);
			Vector3 directionToHandle = (_rightArmTarget.position - _rightArmEndBone.position).normalized;
			directionToHandle.z = 0;
			if (distanceToHandle >= .1f)
			{
				_armatureRoot.position += directionToHandle * 5f * Time.deltaTime;
				if (!_bodyMoved)
				{
					onBodyMoving?.Invoke(directionToHandle, distanceToHandle);
					_bodyMoved = true;
				}
			}
			_headBone.up = _leftArmTarget.position + new Vector3(0, .5f, 0) - _headBone.position;
		}
		

	}


	private void OnUngripRight()
	{
		if (rightArmGrip)
		{
			_startingRightPosition = _rightArmEndBone.position;
			rightArmGrip = false;
		}

	}

	private void OnGripRight()
	{
		rightArmGrip = true;
		_bodyMoved = false;
	}

	private void OnUngripLeft()
	{
		if (leftArmGrip)
		{
			leftArmGrip = false;
			_startingLeftPosition = _leftArmEndBone.position;
		}
		
	}

	private void OnGripLeft()
	{
		leftArmGrip = true;
		_bodyMoved = false;
	}

}
