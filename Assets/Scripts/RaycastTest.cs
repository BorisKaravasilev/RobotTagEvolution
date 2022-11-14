using UnityEngine;

public class RaycastTest : MonoBehaviour
{
    // See Order of Execution for Event Functions for information on FixedUpdate() and Update() related to physics queries
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
    }
}
