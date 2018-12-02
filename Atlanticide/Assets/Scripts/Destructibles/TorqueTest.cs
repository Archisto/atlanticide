using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorqueTest : MonoBehaviour {

    private Rigidbody _rigidBody;
    public Vector3 torqueVector3;
    public ForceMode forceMode;
    public GameObject centerOfMass;

	// Use this for initialization
	void Start () {
        _rigidBody = GetComponent<Rigidbody>();
        _rigidBody.centerOfMass = centerOfMass.transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
        _rigidBody.AddRelativeTorque(torqueVector3, forceMode);
	}
}
