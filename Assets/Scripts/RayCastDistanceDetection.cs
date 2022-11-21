using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastDistanceDetection : MonoBehaviour
{
    RaycastHit hit;
    private float distance;
    // Start is called before the first frame update
  
    void FixedUpdate()

    {
        

        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        Vector3 forwardEndPoint = transform.TransformDirection(Vector3.forward) * 10;
        

        if (Physics.Raycast(transform.position, fwd, 10))
        {
            Debug.DrawRay(transform.position, forwardEndPoint, Color.red);
        }
        else
        {
            Debug.DrawRay(transform.position, forwardEndPoint, Color.green);
        }

        distance = hit.distance;
        Debug.Log(distance);
        




    }

}
