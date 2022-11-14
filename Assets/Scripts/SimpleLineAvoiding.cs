using UnityEngine;
using System.Collections.Generic;

public class SimpleLineAvoiding : MonoBehaviour
{
    // Public fields (exposed to inspector)
    public List<AxleInfo> axleInfos; // the information about each individual axle
    public float motorTorque;
    public List<Transform> sensorTransforms;

    // Private fields
    private float leftMotorTorque = 0f; // <-- Update me to a value between 0.0f - 1.0f
    private float rightMotorTorque = 0f; // <-- me too (if you want more speed then increase "motorTorque" in the inspector
    private bool[] sensorValues;
    int layer_mask;

    /// <summary>
    /// Controller logic.
    /// </summary>
    private void RobotController()
    {
        // Write your code here
        // ...

        if (sensorValues[0] == false)
        {
            leftMotorTorque = 1f; 
            rightMotorTorque = 1f;
        }
        else
        {
            leftMotorTorque = 0f;
            rightMotorTorque = 0f;
        }
    }

    // Executes once in the beginning (good for initialization)
    public void Start()
    {
        sensorValues = new bool[sensorTransforms.Count];
        layer_mask = LayerMask.GetMask("Tape");
    }

    // Executes every frame
    public void Update()
    {
    }

    // Executes over a fixed period of time (good for physics calculations)
    public void FixedUpdate()
    {
        EvaluateRaycasts();
        RobotController();

        foreach (AxleInfo axleInfo in axleInfos)
        {
            // We steer by difference in motor speeds

            //if (axleInfo.steering)
            //{
            //    axleInfo.leftWheel.steerAngle = 0;
            //    axleInfo.rightWheel.steerAngle = 0;
            //}
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = leftMotorTorque;
                axleInfo.rightWheel.motorTorque = rightMotorTorque;
            }

            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }
    }

    // finds the corresponding visual wheel
    // correctly applies the transform
    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

    /// <summary>
    /// Collects the sensor values to an array.
    /// </summary>
    private void EvaluateRaycasts()
    {
        int i = 0;

        foreach (Transform sensorTransform in sensorTransforms)
        {
            Vector3 fwd = sensorTransform.TransformDirection(Vector3.forward);
            Vector3 forwardEndPoint = sensorTransform.TransformDirection(Vector3.forward) * 10;

            if (Physics.Raycast(sensorTransform.position, fwd, 5, layer_mask))
            {
                Debug.DrawRay(sensorTransform.position, forwardEndPoint, Color.red);
                sensorValues[i] = true;
            }
            else
            {
                Debug.DrawRay(sensorTransform.position, forwardEndPoint, Color.green);
                sensorValues[i] = false;
            }

            i++;
        }
    }
}