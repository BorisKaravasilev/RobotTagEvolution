using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KeyboardController : MonoBehaviour
{
    public List<AxleInfo> axleInfos; // the information about each individual axle
    public float motorTorque;

    public KeyCode accelerator;
    public KeyCode breaking;
    public KeyCode steerLeft;
    public KeyCode steerRight;

    private float leftMotorTorque;
    private float rightMotorTorque;

    public void Update()
    {
        float accelerate = Input.GetKey(accelerator) ? 1f : 0f;
        accelerate = Input.GetKey(breaking) ? -1f : accelerate;
        float accelerateLeft = Input.GetKey(steerLeft) ? 1f : 0f;
        float accelerateRight = Input.GetKey(steerRight) ? 1f : 0f;

        leftMotorTorque = motorTorque * accelerate + motorTorque * accelerateRight;
        rightMotorTorque = motorTorque * accelerate + motorTorque * accelerateLeft;
    }

    public void FixedUpdate()
    {
        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = 0;
                axleInfo.rightWheel.steerAngle = 0;
            }
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
}