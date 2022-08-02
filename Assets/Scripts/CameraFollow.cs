using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject football;
    private Vector3 offset;

    void Start()
    {
        offset = transform.position - football.transform.position;
    }

    void LateUpdate()
    {
        transform.position = football.transform.position + offset;
    }
}
