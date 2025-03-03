using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clavicle_IK : MonoBehaviour
{

    [SerializeField]
    private Transform _handIKTarget;

    [SerializeField]
    private LeftOrRight _clavicleSide;

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = (_handIKTarget.position - transform.position).normalized;


        Quaternion tempRotation = Quaternion.LookRotation(transform.forward, direction);
        if(_clavicleSide == LeftOrRight.Left)
            tempRotation = Quaternion.Euler(tempRotation.eulerAngles.x, tempRotation.eulerAngles.y, Mathf.Clamp(tempRotation.eulerAngles.z,55,110));
        else
            tempRotation = Quaternion.Euler(tempRotation.eulerAngles.x, tempRotation.eulerAngles.y, Mathf.Clamp(tempRotation.eulerAngles.z, 250, 305));
        transform.rotation = tempRotation;
    }



}
