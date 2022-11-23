using UnityEngine;
using System.Collections.Generic;

public class ThymioAvoider : Thymio
{
    protected override void RobotController()
    {
        AvoidPerimeter();
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
}
