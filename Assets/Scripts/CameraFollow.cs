using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject football, footballFG;
    private Vector3 offset;
    private bool followFG = false;

    void Start()
    {
        offset = transform.position - football.transform.position;
    }

    void LateUpdate()
    {
        if (followFG)
        {
            transform.position = footballFG.transform.position + offset;
        }
        else
        {
            transform.position = football.transform.position + offset;
        }
    }

    public void Switch()
    {
        followFG = !followFG;
    }
}
