using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToPrise : MonoBehaviour
{
	public bool colliding = false;
	public Transform previousHandle;
	public Transform currentHandle;
	public LeftOrRight handSide;

	[SerializeField]
	private HandTargetFollow handTarget;

	public delegate void OnHandleReached(Transform currentHandle, LeftOrRight side);
	public static OnHandleReached onHandleReached;


	[SerializeField]
	LayerMask _layerMask;

	[SerializeField]
	public Transform _startingPos;
	//private void OnTriggerEnter(Collider other)
	//{
	//	if (other.CompareTag("Prise"))
	//	{
	//		previousHandle = currentHandle;
	//		currentHandle = other.gameObject.transform;
	//		colliding = true;

	//		onHandleReached?.Invoke(currentHandle, handSide);
	//	}
	//}

	//private void OnTriggerExit(Collider other)
	//{
	//	if (other.CompareTag("Prise"))
	//	{
	//		colliding = false;
	//	}
	//}

	private void Update()
	{
		RaycastHit hit;
		if (Physics.Linecast(transform.position, _startingPos.transform.position, out hit, _layerMask))
		{
			if (hit.transform.gameObject.CompareTag("Prise"))
			{
				previousHandle = currentHandle;
				currentHandle = hit.transform;
				//colliding = true;

				onHandleReached?.Invoke(currentHandle, handSide);
			}
			
		}
	}
}

public enum LeftOrRight
{
	Left,
	Right
}
