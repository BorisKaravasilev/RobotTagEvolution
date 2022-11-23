using UnityEngine;
using System.Collections.Generic;

public abstract class Thymio : MonoBehaviour
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

    #region Protected fields (exposed to derived classes)
    // ============================================ Protected fields (exposed to derived classes)
    
    // Motor Values
    protected float leftMotorTorque = 0f; // <-- Update me to a value between 0.0f - 1.0f
    protected float rightMotorTorque = 0f; // <-- me too (if you want more speed then increase "motorTorque" in the inspector

    // Sensor Values
    protected LightSensorValue leftLightSensorValue;  // on the bottom
    protected LightSensorValue rightLightSensorValue; // on the bottom
    
    protected float LeftLeftDistanceSensorValue;
    protected float LeftDistanceSensorValue;
    protected float MiddleDistanceSensorValue;
    protected float RightDistanceSensorValue;
    protected float RightRightDistanceSensorValue;

    protected float BackLeftDistanceSensorValue;
    protected float BackRightDistanceSensorValue;

    #endregion
    
    #region Private fields
    // ============================================ Private fields
    
    private int layer_mask_perimeter;
    private int layer_mask_safeZone;
    
    #endregion
    
    protected enum LightSensorValue {
        Perimeter,
        SafeZone,
        Nothing
    }

    /// <summary>
    /// Controller logic setting the torques of the motors based on sensor input.
    /// </summary>
    protected abstract void RobotController();
    
    /// <summary>
    /// Updates the color of LEDs based on sensor input.
    /// </summary>
    protected abstract void UpdateLEDColor();

    /// <summary>
    /// Handles avoiding of the edge of the arena.
    /// </summary>
    /// <returns>Returns true if the robot is on the edge of the arena.</returns>
    protected bool AvoidPerimeter()
    {
        bool avoidingPerimeter = true;
        
        // Motor Speeds
        if (leftLightSensorValue == LightSensorValue.Perimeter && 
            rightLightSensorValue == LightSensorValue.Perimeter)
        {
            leftMotorTorque = -1f;
            rightMotorTorque = -1f;
        }

        if (leftLightSensorValue == LightSensorValue.Perimeter &&
            rightLightSensorValue != LightSensorValue.Perimeter)
        {
            leftMotorTorque = 1f;
            rightMotorTorque = -1f;
        }

        if (leftLightSensorValue != LightSensorValue.Perimeter &&
            rightLightSensorValue == LightSensorValue.Perimeter)
        {
            leftMotorTorque = -1f;
            rightMotorTorque = 1f;
        }

        if (leftLightSensorValue != LightSensorValue.Perimeter &&
            rightLightSensorValue != LightSensorValue.Perimeter)
        {
            leftMotorTorque = 1f;
            rightMotorTorque = 1f;
            avoidingPerimeter = false;
        }

        return avoidingPerimeter;
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
        RobotController();
        UpdateLEDColor();
        ApplyTorqueToMotors();
    }

    private void ApplyTorqueToMotors()
    {
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
    private void ApplyLocalPositionToVisuals(WheelCollider collider)
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

    private void ReadDistanceSensors()
    {
        // TODO: Balazs integrates his code here (feel free to take inspiration from the method above)
    }
}
