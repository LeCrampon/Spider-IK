using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HandTargetFollow : MonoBehaviour
{
    [SerializeField]
    private GoToPrise stickTarget;

	private Transform nextHandle;
	public LeftOrRight handSide;

	[SerializeField]
	Transform _handBone;

	[SerializeField]
	private float _handHeight = .2f;


	private void OnEnable()
	{
		GoToPrise.onHandleReached += SetNextHandle;
	}

	private void Update()
	{
		if(nextHandle !=null && !IsNextHandleReached())
		{
			Vector3 direction = CalculateNextHandleDirection();
			transform.position += direction * 5 * Time.deltaTime;

			if (handSide == LeftOrRight.Left)
			{
				//_handBone.rotation = nextHandle.rotation * Quaternion.Euler(0, 90, 0);
				_handBone.localRotation = Quaternion.Euler(0, 90, nextHandle.rotation.eulerAngles.z);
			}
			else
			{
				//_handBone.rotation = nextHandle.rotation * Quaternion.Euler(0, -90, 0);
				_handBone.localRotation = Quaternion.Euler(90, -90, nextHandle.rotation.eulerAngles.z);
			}
		}
	}



	private void SetNextHandle(Transform handle, LeftOrRight side)
	{
		if(handSide == side)
			nextHandle = handle;
	}

	private bool IsNextHandleReached()
	{
		return Vector3.Distance((nextHandle.position + Vector3.down * _handHeight), transform.position) < .01f;
	}

	private Vector3 CalculateNextHandleDirection()
	{
		return ((nextHandle.position + Vector3.down * _handHeight) - transform.position).normalized;
	}
}
