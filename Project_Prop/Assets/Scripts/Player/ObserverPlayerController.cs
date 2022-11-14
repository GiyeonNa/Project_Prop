using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObserverPlayerController : MonoBehaviour
{
    //public List<Camera> cameras;
    public Camera[] cameras;
    public int cameraIndex = 0;

    private void Start()
    {
        cameras = GameObject.FindObjectsOfType<Camera>(true);
        SwitchCamera();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (cameraIndex == cameras.Length || cameras[cameraIndex] is null) cameraIndex = 0;
            SwitchCamera();
        }
    }

    private void SwitchCamera()
    {
        cameras[cameraIndex].gameObject.SetActive(true);
        cameraIndex++;
    }
}
