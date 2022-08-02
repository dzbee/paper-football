using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public GameObject overheadCamera;
    public GameObject tableCamera;
    
    void Start() {
        overheadCamera.SetActive(true);
        tableCamera.SetActive(false);
    }

    void Update() {
        if(Input.GetButtonDown("Swap")){
            overheadCamera.SetActive(!overheadCamera.activeInHierarchy);
            tableCamera.SetActive(!tableCamera.activeInHierarchy);
        }
    }
}
