using UnityEngine;
using System.Collections.Generic;

public class ThymioAvoider : Thymio
{
    protected override void RobotController()
    {
        // leftMotorTorque = (float) Chromosome[0];
        // rightMotorTorque = (float) Chromosome[1];
        //
        // AvoidPerimeter();
        updateFitness();

        double leftSensorInput = leftLightSensorValue == LightSensorValue.Perimeter ? 0 : 1;
        double rightSensorInput = rightLightSensorValue == LightSensorValue.Perimeter ? 0 : 1;

        // var testInputs = new[] { leftSensorInput, rightSensorInput };
        // var output = _neuralNetwork.ComputeOutput(testInputs);
        
        leftMotorTorque = (float) (leftSensorInput * chromosome[0] + chromosome[2]);
        rightMotorTorque =  (float) (rightSensorInput * chromosome[1] + chromosome[3]);
        
        // print("Output: " + output);

        if (transform.position.y < -1f)
        {
            Tagged = true;
            _fitness = _fitness / 2;
        }
    }

    protected override void UpdateLEDColor()
    {
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
    }

    public override void updateFitness()
    {
        if (!Tagged || !(leftLightSensorValue == LightSensorValue.Perimeter || rightLightSensorValue == LightSensorValue.Perimeter))
        {
            // if (leftLightSensorValue == LightSensorValue.SafeZone && rightLightSensorValue == LightSensorValue.SafeZone)
            // {
            //     IncrementFitness(Time.deltaTime * 9);
            // }
            
            IncrementFitness(Time.deltaTime * (leftMotorTorque + rightMotorTorque));
        }
    }
}
