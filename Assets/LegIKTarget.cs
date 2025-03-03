using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegIKTarget : MonoBehaviour
{
    public LeftOrRight legSide;
	public LayerMask layerMask;

	[SerializeField]
	private Transform _legEndBone;
	public Transform nextHandle;
	public Transform previousHandle;


	[SerializeField]
	private Transform _hipBone;


	[SerializeField]
	private LegIKTarget _otherLeg;

	//EVENT
	public delegate void OnLeftLegReached(Vector3 direction, float distance);
	public static OnLeftLegReached onLeftLegReached;

	public Vector3 movDirection;
	public float movDistance;

	[SerializeField]
	private float _legLength;

	[SerializeField]
	private float _feetHeight = .4f;

	private void OnEnable()
	{
		if (legSide == LeftOrRight.Left)
			MovingIKScript.onBodyMoving += SetNextFootHandle;
		else
			onLeftLegReached += SetNextFootHandle;
	}

	private void Start()
	{
		_legLength = _legEndBone.GetComponent<SimpleIK>()._fullLength;

	}

	void SearchNextTarget(Vector3 direction, float distance)
	{
		_otherLeg.movDirection = direction;
		_otherLeg.movDistance = distance;

		RayCastForLegTarget();

	}

	private void RayCastForLegTarget()
	{
		nextHandle = null;
		for(float xPosition = -2; xPosition < 1; xPosition += .25f)
		{

			if (legSide == LeftOrRight.Left)
			{
				RaycastHit hit;
				if (Physics.Raycast(_hipBone.position, new Vector3(xPosition, -1, 0), out hit, _legLength, layerMask))
				{
					previousHandle = nextHandle;
					nextHandle = hit.transform;
					Debug.DrawLine(_hipBone.position, hit.point, Color.green, 1.5f);
				}

				Debug.DrawRay(_hipBone.position, new Vector3(xPosition, -1, 0), Color.cyan, 1.5f);
			}
			else
			{
				RaycastHit hit;
				if (Physics.Raycast(_hipBone.position, new Vector3(-xPosition, -1, 0), out hit, _legLength, layerMask))
				{
					previousHandle = nextHandle;
					nextHandle = hit.transform;
					Debug.DrawLine(_hipBone.position, hit.point, Color.green, 1.5f);
				}
				Debug.DrawRay(_hipBone.position, new Vector3(xPosition, -1, 0), Color.cyan, 1.5f);
			}
		}
	}

	private void SetNextFootHandle(Vector3 direction, float distance)
	{
		if (legSide == LeftOrRight.Left)
		{
			SearchNextTarget(direction, distance);
			Debug.Log("LEFT FOOT HANDLE TRIGGERED");
		}
		else if (legSide == LeftOrRight.Right) 
		{
			SearchNextTarget(direction, distance);
			Debug.Log("RIGHT FOOT HANDLE TRIGGERED");
		}


	}

	private void Update()
	{
		if(nextHandle == null && previousHandle != null && IsPreviousHandleInReach())
		{
			RayCastForLegTarget();
		}

		if(nextHandle != null && !IsNextHandleReached())
		{
			Vector3 handleDirection = ((nextHandle.position + Vector3.up * _feetHeight)  - transform.position).normalized;
			transform.position += handleDirection * 5 * Time.deltaTime;
		}
		else if (nextHandle != null && IsNextHandleReached())
		{
			nextHandle = null;
			if(legSide == LeftOrRight.Left)
				LegIKTarget.onLeftLegReached?.Invoke(movDirection, movDistance);
		}
	}

	private bool IsNextHandleReached()
	{
		return Vector3.Distance(nextHandle.position + Vector3.up * _feetHeight, transform.position) < .01f;
	}

	private bool IsPreviousHandleInReach()
	{
		return Vector3.Distance(previousHandle.position + Vector3.up * _feetHeight, transform.position) < .01f;
	}
}
