using UnityEngine;
using System.Collections.Generic;

public class ThymioAvoider : Thymio
{
    protected override void RobotController()
    {
        AvoidPerimeter();
        updateFitness();

        leftMotorTorque = (float) Chromosome[0];
        rightMotorTorque = (float) Chromosome[1];

        if (transform.position.y < -1f) Tagged = true;
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
        if (!Tagged)
        {
            IncrementFitness(Time.deltaTime);
        }
    }
}
