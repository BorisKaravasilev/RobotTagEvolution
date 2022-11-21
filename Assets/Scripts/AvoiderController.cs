using UnityEngine;
using System.Collections.Generic;

public enum SensorValue {
    Perimeter,
    SafeZone,
    Nothing
}

public class AvoiderController : MonoBehaviour
{
    // Public fields (exposed to inspector)
    public List<AxleInfo> axleInfos; // the information about each individual axle
    public float motorTorque;
    public List<Transform> sensorTransforms;
    public bool Tagged = false;

    [Header("LED Visualization")]
    public Renderer LEDRenderer;
    public Material AvoidingColor;
    public Material TaggedColor;
    public Material InSafeZoneColor;

    // Private fields
    private float leftMotorTorque = 0f; // <-- Update me to a value between 0.0f - 1.0f
    private float rightMotorTorque = 0f; // <-- me too (if you want more speed then increase "motorTorque" in the inspector
    private SensorValue[] sensorValues;
    private int layer_mask_perimeter;
    private int layer_mask_safeZone;

    /// <summary>
    /// Controller logic.
    /// </summary>
    private void RobotController()
    {
        // Setting LED colors
        if (Tagged)
        {
            LEDRenderer.material = TaggedColor;
        }
        else if (sensorValues[0] == SensorValue.Nothing)
        {
            LEDRenderer.material = AvoidingColor;
        }
        else if (sensorValues[0] == SensorValue.SafeZone)
        {
            LEDRenderer.material = InSafeZoneColor;
        }

        if (sensorValues[0] != SensorValue.Perimeter )
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
        sensorValues = new SensorValue[sensorTransforms.Count];
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
                axleInfo.leftWheel.motorTorque = leftMotorTorque * motorTorque;
                axleInfo.rightWheel.motorTorque = rightMotorTorque * motorTorque;
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

            if (Physics.Raycast(sensorTransform.position, fwd, 5, layer_mask_perimeter))
            {
                Debug.DrawRay(sensorTransform.position, forwardEndPoint, Color.red);
                sensorValues[i] = SensorValue.Perimeter;
            }
            else if (Physics.Raycast(sensorTransform.position, fwd, 5, layer_mask_safeZone))
            {
                Debug.DrawRay(sensorTransform.position, forwardEndPoint, Color.green);
                sensorValues[i] = SensorValue.SafeZone;
            }
            else
            {
                Debug.DrawRay(sensorTransform.position, forwardEndPoint, Color.white);
                sensorValues[i] = SensorValue.Nothing;
            }

            i++;
        }
    }
}