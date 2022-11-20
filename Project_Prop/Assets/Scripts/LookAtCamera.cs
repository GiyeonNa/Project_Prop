using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.UIElements;

public class LookAtCamera : MonoBehaviour
{
    public Transform target;
    Transform hitTransform;
    RaycastHit hit;
    [SerializeField] LayerMask layermask;

    // Update is called once per frame
    void Update()
    { 
        transform.LookAt(target);
        Vector3 dir = target.position - transform.position;
        Debug.DrawRay(transform.position, dir, Color.red);

        if(Physics.Raycast(transform.position, dir, out hit, layermask))
        {
            
            Color cor;
            if (hit.transform != hitTransform)
            {
                if(hitTransform != null)
                {
                    #region 다시 불투명하게
                    cor = hitTransform.GetComponent<Renderer>().material.color;
                    cor.a = 1;
                    hitTransform.GetComponent<Renderer>().material.color = cor;
                    #endregion
                }
            }
            hitTransform = hit.transform;
            
            #region 투명하게
            cor = hit.transform.GetComponent<Renderer>().material.color;
            cor.a = 0.1f;
            hit.transform.GetComponent<Renderer>().material.color = cor;
            #endregion
        }

    }
}
