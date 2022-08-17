using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    [SerializeField] GameObject overheadCamera, tableCamera;
    Quaternion overheadOrientation, tablesideOrientation;
    Vector3 tablesidePosition;
    float rotateTime = 0.5f;
    float rotationTimeElapsed = 0;
    float tablesideDistance;

    void Start()
    {
        overheadCamera.SetActive(true);
        tableCamera.SetActive(false);
    }

    void SwitchCamera()
    {
        overheadCamera.SetActive(!overheadCamera.activeInHierarchy);
        tableCamera.SetActive(!tableCamera.activeInHierarchy);
    }

    Vector3 MoveTableCameraRight(Vector3 currentPosition) {
        if (currentPosition.x == tablesideDistance)
        {
            // Camera is east of the table, move north
            return new Vector3(0, currentPosition.y, tablesideDistance);
        }
        else if (currentPosition.z == tablesideDistance)
        {
            // Camera is north of the table, move west
            return new Vector3(-tablesideDistance, currentPosition.y, 0);
        }
        else if (currentPosition.x == -tablesideDistance)
        {
            // Camera is west of the table, move south
            return new Vector3(0, currentPosition.y, -tablesideDistance);
        }
        // Camera is south of the table, move east
        return new Vector3(tablesideDistance, currentPosition.y, 0);
    }

    Vector3 MoveTableCameraLeft(Vector3 currentPosition) {
        if (currentPosition.x == tablesideDistance)
        {
            // Camera is east of the table, move south
            return new Vector3(0, currentPosition.y, -tablesideDistance);
        }
        else if (currentPosition.z == tablesideDistance)
        {
            // Camera is north of the table, move east
            return new Vector3(tablesideDistance, currentPosition.y, 0);
        }
        else if (currentPosition.x == -tablesideDistance)
        {
            // Camera is west of the table, move north
            return new Vector3(0, currentPosition.y, tablesideDistance);
        }
        // Camera is south of the table, move west
        return new Vector3(-tablesideDistance, currentPosition.y, 0);
    }

    IEnumerator RotateCameras(float degrees)
    {
        overheadOrientation = overheadCamera.transform.rotation;
        tablesideOrientation = tableCamera.transform.rotation;
        tablesidePosition = tableCamera.transform.position;
        Vector3 nextTablesidePosition = MoveTableCameraRight(tablesidePosition);
        if (degrees > 0)
        {
            nextTablesidePosition = MoveTableCameraLeft(tablesidePosition);
        }

        while (rotationTimeElapsed < rotateTime)
        {
            rotationTimeElapsed += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
            overheadCamera.transform.rotation = Quaternion.Lerp(overheadOrientation, Quaternion.Euler(0, degrees, 0) * overheadOrientation, rotationTimeElapsed / rotateTime);
            tableCamera.transform.rotation = Quaternion.Slerp(tablesideOrientation, Quaternion.Euler(0, degrees, 0) * tablesideOrientation, rotationTimeElapsed / rotateTime);
            tableCamera.transform.position = Vector3.Slerp(tablesidePosition, nextTablesidePosition, rotationTimeElapsed / rotateTime);
        }
        rotationTimeElapsed = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchCamera();
        }
        if (Input.GetKeyDown(KeyCode.E) & rotationTimeElapsed == 0)
        {
            StartCoroutine(RotateCameras(-90));
        }
        else if (Input.GetKeyDown(KeyCode.Q) & rotationTimeElapsed == 0)
        {
            StartCoroutine(RotateCameras(90));
        }
    }

    void Awake()
    {
        tablesideDistance = Mathf.Abs(tableCamera.transform.position.z);
    }
}
