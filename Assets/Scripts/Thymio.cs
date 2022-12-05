using UnityEngine;
using System.Collections.Generic;

public abstract class Thymio : MonoBehaviour
{
    #region Public fields (exposed to inspector)
    // ============================================ Public fields (exposed to inspector)

    public bool ShowDebugVisuals = true;
    public bool Tagged = false;
    
    public float RaspberryPiCameraFOV = 62f; // field of view in degrees
    public float cameraRange = 14; // in meters
    public Transform RaspberryPiCamera;
    
    [Header("Motors")]
    public List<AxleInfo> AxleInfos; // the information about each individual axle
    public float MotorTorque;
    
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

    protected List<Thymio> RobotsInFOV = new List<Thymio>();

    #endregion
    
    #region Private fields
    // ============================================ Private fields
    
    private int _layerMaskPerimeter;
    private int _layerMaskSafeZone;
    private Thymio[] _robotsInArena;
    private double _fitness;

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
    /// Increments the fitness of the individual with the specified amount (1.0 by default).
    /// </summary>
    /// <param name="amount"></param>
    protected void IncrementFitness(double amount = 1.0)
    {
        _fitness += amount;
    }

    /// <summary>
    /// Returns the fitness of the individual.
    /// </summary>
    protected double GetFitness()
    {
        return _fitness;
    }

    public void SetRobotsInArena(Thymio[] robotsInArena)
    {
        this._robotsInArena = robotsInArena;
    }

    // Executes once in the beginning (good for initialization)
    public void Start()
    {
        _layerMaskPerimeter = LayerMask.GetMask("Perimeter");
        _layerMaskSafeZone = LayerMask.GetMask("SafeZone");
        LEDRenderer.material = AvoidingColor;
        _robotsInArena = FindObjectsOfType<Thymio>();
    }

    // Executes every frame
    public void Update()
    {
        ShowFieldOfView();
    }

    // Executes over a fixed period of time (good for physics calculations)
    public void FixedUpdate()
    {
        ReadLightSensors();
        GetRobotPositionsFromCamera();
        RobotController();
        UpdateLEDColor();
        ApplyTorqueToMotors();
    }
    
    private void GetRobotPositionsFromCamera()
    {
        RobotsInFOV.Clear();
        
        foreach (Thymio robot in _robotsInArena)
        {
            if (!robot.enabled || robot == this) continue; // Early out if game object is disabled or itself (disabled != robot is tagged)

            Vector3 robotPosition = robot.transform.position;
            robotPosition.y = RaspberryPiCamera.position.y; // Pretend that the robot is in the height of the camera

            Vector3 vectorToRobot = robotPosition - RaspberryPiCamera.position;
            
            float angleFromCenter = Vector3.Angle(RaspberryPiCamera.forward, vectorToRobot);
            float halfFOVAngle = RaspberryPiCameraFOV / 2f;
            bool robotInFOV = angleFromCenter <= halfFOVAngle && vectorToRobot.magnitude <= cameraRange;
            
            if (robotInFOV && ShowDebugVisuals)
            {
                Debug.DrawLine(RaspberryPiCamera.position, robotPosition, robot.LEDRenderer.material.color);
                RobotsInFOV.Add(robot);
            }
        }
    }

    /// <summary>
    /// Visualizes the field of view if "ShowDebugVisuals" is enabled.
    /// </summary>
    private void ShowFieldOfView()
    {
        if (ShowDebugVisuals)
        {
            Vector3 leftFOVLimit = RaspberryPiCamera.TransformDirection(Vector3.forward) * cameraRange;
            leftFOVLimit = Quaternion.Euler(0, -RaspberryPiCameraFOV / 2f, 0) * leftFOVLimit;
            Debug.DrawRay(RaspberryPiCamera.position, leftFOVLimit, Color.white);
            
            Vector3 rightFOVLimit = RaspberryPiCamera.TransformDirection(Vector3.forward) * cameraRange;
            rightFOVLimit = Quaternion.Euler(0, RaspberryPiCameraFOV / 2f, 0) * rightFOVLimit;
            Debug.DrawRay(RaspberryPiCamera.position, rightFOVLimit, Color.white);
        }
    }
    
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

    private void ApplyTorqueToMotors()
    {
        foreach (AxleInfo axleInfo in AxleInfos)
        {
            // We steer by difference in motor speeds therefore we don't need a steering axel

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
            Vector3 forwardEndPoint = sensorTransform.TransformDirection(Vector3.forward) * 3;

            if (Physics.Raycast(sensorTransform.position, fwd, 5, _layerMaskPerimeter))
            {
                if (ShowDebugVisuals) Debug.DrawRay(sensorTransform.position, forwardEndPoint, Color.red);
                return LightSensorValue.Perimeter;
            }
            
            if (Physics.Raycast(sensorTransform.position, fwd, 5, _layerMaskSafeZone))
            {
                if (ShowDebugVisuals) Debug.DrawRay(sensorTransform.position, forwardEndPoint, Color.green);
                return LightSensorValue.SafeZone;
            }

            if (ShowDebugVisuals) Debug.DrawRay(sensorTransform.position, forwardEndPoint, Color.white);
            return LightSensorValue.Nothing;
        }
    }

    private void ReadDistanceSensors()
    {
        // TODO: Balazs integrates his code here (feel free to take inspiration from the method above)
    }
}
