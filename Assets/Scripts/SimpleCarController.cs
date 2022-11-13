using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleCarController : MonoBehaviour
{
    public List<AxleInfo> axleInfos; // the information about each individual axle
    public float leftMotorSpeed;
    public float rightMotorSpeed;

    private float leftMotorCurrentSpeed;
    private float rightMotorCurrentSpeed;

    public void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            leftMotorCurrentSpeed = leftMotorSpeed;
        }
        else
        {
            leftMotorCurrentSpeed = 0;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            rightMotorCurrentSpeed = rightMotorSpeed;
        }
        else
        {
            rightMotorCurrentSpeed = 0;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            leftMotorCurrentSpeed = -leftMotorCurrentSpeed;
            rightMotorCurrentSpeed = -rightMotorCurrentSpeed;
        }
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
                axleInfo.leftWheel.motorTorque = leftMotorCurrentSpeed;
                axleInfo.rightWheel.motorTorque = rightMotorCurrentSpeed;
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

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor; // is this wheel attached to motor?
    public bool steering; // does this wheel apply steer angle?
}
