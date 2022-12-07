using UnityEngine;
using System.Collections.Generic;

public enum LightSensorValue {
    Perimeter,
    SafeZone,
    Nothing
}

public class AvoiderController : MonoBehaviour
{
    #region Public fields (exposed to inspector)
    // ============================================ Public fields (exposed to inspector)
    
    public List<AxleInfo> AxleInfos; // the information about each individual axle
    public float MotorTorque;
    public bool Tagged = false;

    [Header("Sensor Transforms")]
    public Transform LeftLightSensor;  // on the bottom
    public Transform RightLightSensor; // on the bottom
    [Space]
    public Transform LeftLeftDistanceSensor;
    public Transform LeftDistanceSensor;
    public Transform MiddleDistanceSensor;
    public Transform RightDistanceSensor;
    public Transform RightRightDistanceSensor;
    [Space]
    public Transform BackLeftDistanceSensor;
    public Transform BackRightDistanceSensor;
    
    [Header("LED Visualization")]
    public Renderer LEDRenderer;
    public Material AvoidingColor;
    public Material TaggedColor;
    public Material InSafeZoneColor;

    #endregion

    #region Private fields
    // ============================================ Private fields
    
    private float leftMotorTorque = 0f; // <-- Update me to a value between 0.0f - 1.0f
    private float rightMotorTorque = 0f; // <-- me too (if you want more speed then increase "motorTorque" in the inspector

    private int layer_mask_perimeter;
    private int layer_mask_safeZone;
    
    // Sensor Values
    private LightSensorValue leftLightSensorValue;  // on the bottom
    private LightSensorValue rightLightSensorValue; // on the bottom
    
    private float LeftLeftDistanceSensorValue;
    private float LeftDistanceSensorValue;
    private float MiddleDistanceSensorValue;
    private float RightDistanceSensorValue;
    private float RightRightDistanceSensorValue;

    private float BackLeftDistanceSensorValue;
    private float BackRightDistanceSensorValue;

    
    

    #endregion


    /// <summary>
    /// Controller logic.
    /// </summary>
    private void RobotController()
    {
        // LED Color
        if (Tagged)
        {
            LEDRenderer.material = TaggedColor;
        }
        else if (leftLightSensorValue == LightSensorValue.SafeZone && 
                 rightLightSensorValue == LightSensorValue.SafeZone)
        {
            LEDRenderer.material = InSafeZoneColor;
        }
        else
        {
            LEDRenderer.material = AvoidingColor;
        }

        // Motor Speeds
        if (leftLightSensorValue == LightSensorValue.Perimeter && 
            rightLightSensorValue == LightSensorValue.Perimeter)
        {
            leftMotorTorque = -1f;
            rightMotorTorque = -1f;
        }
        else if (leftLightSensorValue == LightSensorValue.Perimeter &&
                   rightLightSensorValue != LightSensorValue.Perimeter)
        {
            leftMotorTorque = 1f;
            rightMotorTorque = -1f;
        }
        else if (leftLightSensorValue != LightSensorValue.Perimeter &&
                   rightLightSensorValue == LightSensorValue.Perimeter)
        {
            leftMotorTorque = -1f;
            rightMotorTorque = 1f;
        }
        else
        {
            leftMotorTorque = 1f;
            rightMotorTorque = 1f;
            print("Driving forward");
        }

        if(LeftLeftDistanceSensorValue != 0f && LeftLeftDistanceSensorValue < 5f)
        {
            leftMotorTorque = 1f;
            rightMotorTorque = 0.5f;
        }
    }

    // Executes once in the beginning (good for initialization)
    public void Start()
    {
        layer_mask_perimeter = LayerMask.GetMask("Perimeter");
        layer_mask_safeZone = LayerMask.GetMask("SafeZone");
        LEDRenderer.material = AvoidingColor;
    }

    // Executes every frame
    public void Update()
    {
    }

    // Executes over a fixed period of time (good for physics calculations)
    public void FixedUpdate()
    {
        ReadLightSensors();
        ReadDistance();
        RobotController();
        
        foreach (AxleInfo axleInfo in AxleInfos)
        {
            // We steer by difference in motor speeds

            //if (axleInfo.steering)
            //{
            //    axleInfo.leftWheel.steerAngle = 0;
            //    axleInfo.rightWheel.steerAngle = 0;
            //}
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = leftMotorTorque * MotorTorque;
                axleInfo.rightWheel.motorTorque = rightMotorTorque * MotorTorque;
            }

            if (Tagged)
            {
                axleInfo.leftWheel.motorTorque = 0f;
                axleInfo.rightWheel.motorTorque = 0f;
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
    
    private void ReadLightSensors()
    {
        leftLightSensorValue = ReadSensor(LeftLightSensor);
        rightLightSensorValue = ReadSensor(RightLightSensor);
        
        print(leftLightSensorValue);

        // Local function (function in a function)
        LightSensorValue ReadSensor(Transform sensorTransform)
        {
            Vector3 fwd = sensorTransform.TransformDirection(Vector3.forward);
            Vector3 forwardEndPoint = sensorTransform.TransformDirection(Vector3.forward) * 10;

            if (Physics.Raycast(sensorTransform.position, fwd, 5, layer_mask_perimeter))
            {
                Debug.DrawRay(sensorTransform.position, forwardEndPoint, Color.red);
                return LightSensorValue.Perimeter;
            }
            
            if (Physics.Raycast(sensorTransform.position, fwd, 5, layer_mask_safeZone))
            {
                Debug.DrawRay(sensorTransform.position, forwardEndPoint, Color.green);
                return LightSensorValue.SafeZone;
            }

            Debug.DrawRay(sensorTransform.position, forwardEndPoint, Color.white);
            return LightSensorValue.Nothing;
        }
    }

    private void ReadDistance() {
        float distanceLimit = 10;
        LeftLeftDistanceSensorValue = ReadSensorDistance(LeftLeftDistanceSensor);
        LeftDistanceSensorValue = ReadSensorDistance(LeftDistanceSensor);
        MiddleDistanceSensorValue = ReadSensorDistance(MiddleDistanceSensor);
        RightDistanceSensorValue = ReadSensorDistance(RightDistanceSensor);
        RightRightDistanceSensorValue = ReadSensorDistance(RightRightDistanceSensor);
        BackLeftDistanceSensorValue = ReadSensorDistance(BackLeftDistanceSensor);
        BackRightDistanceSensorValue = ReadSensorDistance(BackRightDistanceSensor);

        
        float ReadSensorDistance(Transform sensorTransform)
        {
           
            Ray ray = new Ray(sensorTransform.position, sensorTransform.forward);
            Vector3 forwardEndPoint = sensorTransform.TransformDirection(Vector3.forward) * distanceLimit;
            Debug.DrawRay(sensorTransform.position, forwardEndPoint, Color.green);
            RaycastHit hitData;
            Physics.Raycast(ray, out hitData, distanceLimit);
            Debug.Log(hitData.distance);
            return hitData.distance;
           
        }
    }
}
