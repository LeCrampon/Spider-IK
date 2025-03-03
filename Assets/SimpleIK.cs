using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleIK : MonoBehaviour
{
    [SerializeField]
    private int _armLength;

    [SerializeField]
    private Transform _target;

    [SerializeField]
    private Transform _control;

    //Bones
    private Transform[] _bones;
    private Vector3[] _positions;
    private float[] _boneLengths;
    public float _fullLength;

    [SerializeField]
    private int _iterations = 10;

    private float accuracy = .001f;

    //Rotation thingys
    private Vector3[] _startDirections;
    private Quaternion[] _startRotations;
    private Quaternion _targetStartRotation;

	private void Start()
	{
        InitializeArm();
	}

	private void Update()
	{
        ResolveIK();
	}

	private void OnDrawGizmos()
	{
		Transform current = this.transform;
        for (int i = 0; i < _armLength && current != null && current.parent != null; i++)
        {
            Debug.DrawLine(current.position, current.parent.position, Color.red);
            current = current.parent;
        }
    }

    private void InitializeArm()
	{
        _bones = new Transform[_armLength + 1];
        _positions = new Vector3[_armLength + 1];

        _startDirections = new Vector3[_armLength + 1];
        _startRotations = new Quaternion[_armLength + 1];

        _targetStartRotation = _target.rotation;


        _boneLengths = new float[_armLength];

        _fullLength = 0;

        Transform current = transform;
        for(int i = _bones.Length -1; i >= 0; i--)
		{
            _bones[i] = current;
            _startRotations[i] = current.rotation;

            if(i == _bones.Length - 1)
			{
                _startDirections[i] = _target.position - current.position;
			}
			else
			{
                _boneLengths[i] = (_bones[i + 1].position - current.position).magnitude;
                _fullLength += _boneLengths[i];

                _startDirections[i] = _bones[i + 1].position - current.position;
			}

            current = current.parent;
		}
	}

    //The actual algorithm (FABRIK)
    private void ResolveIK()
	{
        if (_boneLengths.Length != _armLength)
            InitializeArm();

        for(int i =0; i< _bones.Length; i++)
		{
            _positions[i] = _bones[i].position;
		}

        //CALCUL

        float distanceToTargetSQRD = (_target.position - _bones[0].position).sqrMagnitude;

        // Si _target trop loin
        if (distanceToTargetSQRD >= _fullLength * _fullLength)
		{
            Vector3 direction = (_target.position - _positions[0]).normalized;

            for(int i = 1; i < _positions.Length; i++)
			{
                _positions[i] = _positions[i - 1] + direction * _boneLengths[i - 1];
			}
		}
		else
		{
            for(int iter =0; iter<_iterations; iter++)
			{
                //BACK PROPAGATION
                for(int i = _positions.Length -1; i> 0; i--)
				{
                    if(i == _positions.Length - 1)
					{
                        _positions[i] = _target.position;
					}
					else
					{
                        //position d'après plus longueur du bras fois la direction entre les deux bones.
                        _positions[i] = _positions[i + 1] + (_positions[i] - _positions[i + 1]).normalized * _boneLengths[i];
					}
				}
				//FRONT PROPAGATION

				for (int i = 1; i < _positions.Length; i++)
				{
					//la même chose dans l'autre sens.
					_positions[i] = _positions[i - 1] + (_positions[i] - _positions[i - 1]).normalized * _boneLengths[i - 1];
				}

				float squareDistance = (_positions[_positions.Length - 1] - _target.position).sqrMagnitude;
                if (squareDistance < accuracy * accuracy)
                    break;
			}
		}
        //ROTATIONS
        for (int i = 0; i < _positions.Length; i++)
        {
            if (i == _positions.Length - 1)
            {
                _bones[i].rotation = _target.rotation * Quaternion.Inverse(_targetStartRotation) * _startRotations[i];
            }
            else
            {
                _bones[i].rotation = Quaternion.FromToRotation(_startDirections[i], _positions[i + 1] - _positions[i]) * _startRotations[i];
            }
        }


        //POLE TARGET
        if(_control != null)
		{
            //tous sauf le premier et le dernier
            for(int i =1; i < _positions.Length -1; i++)
			{
                //On créé un plan normal au bone
                Plane projectionPlane = new Plane(_positions[i + 1] - _positions[i - 1], _positions[i - 1]);
                //on projette le bonne sur le plan
                Vector3 bonePosProjection = projectionPlane.ClosestPointOnPlane(_positions[i]);
                //on projette le control sur le plan
                Vector3 controlProjection = projectionPlane.ClosestPointOnPlane(_control.position);

                //on calcule l'angle entre les deux projections sur le plan
                float angleOnPlane = Vector3.SignedAngle(bonePosProjection - _positions[i - 1], controlProjection - _positions[i - 1], projectionPlane.normal);

                _positions[i] = Quaternion.AngleAxis(angleOnPlane, projectionPlane.normal) * (_positions[i] - _positions[i - 1]) + _positions[i - 1];
			}
		}

        for (int i = 0; i < _bones.Length; i++)
        {
            _bones[i].position = _positions[i];
		}


        
	}


}
