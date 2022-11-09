using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]

public class Hide : MonoBehaviour
{
    [SerializeField] MeshFilter selfMeshFilter;
    //[SerializeField] SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] Transform debug;
    [SerializeField] LayerMask layerMask;

    private void Awake()
    {
        selfMeshFilter = GetComponent<MeshFilter>();
    }

    void Update()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        
        if (Input.GetKeyUp(KeyCode.E))
        {
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                
                GameObject obj = hit.collider.gameObject;
                Debug.Log(obj.name);
                Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
                //if (skinnedMeshRenderer.enabled) skinnedMeshRenderer.enabled = false;
                selfMeshFilter.mesh = mesh;
            }
        }
            
    }
}