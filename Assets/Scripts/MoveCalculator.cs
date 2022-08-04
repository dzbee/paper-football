using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCalculator : MonoBehaviour
{
    public GameObject football;
    public GameObject table;

    public float Force(Vector3 target) {
        var distance = (target - football.transform.position).magnitude;
        var mass = football.GetComponent<Rigidbody>().mass;
        var footballFriction = football.GetComponent<Collider>().material.dynamicFriction;
        var tableFriction = table.GetComponent<Collider>().material.dynamicFriction;
        var frictionCoef = (footballFriction + tableFriction) / 2;
        var initialVelocity = Mathf.Sqrt(2 * distance * frictionCoef * Physics.gravity.magnitude);
        return initialVelocity * mass * 1.3f;
    }
}
