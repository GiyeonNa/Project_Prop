using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObserverPlayerController : MonoBehaviour
{
    //public List<Camera> cameras;
    public Camera[] cameras;
    public int cameraIndex = 0;
    public Camera tempCamera;

    private void Start()
    {
        cameras = GameObject.FindObjectsOfType<Camera>(true);
        cameras[cameraIndex].gameObject.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            tempCamera = cameras[cameraIndex];
            if (cameraIndex == (cameras.Length - 2)) cameraIndex = -1;
            cameraIndex++;
            SwitchCamera();
        }
    }

    private void SwitchCamera()
    {
        tempCamera.gameObject.SetActive(false);
        cameras[cameraIndex].gameObject.SetActive(true);
    }
}
