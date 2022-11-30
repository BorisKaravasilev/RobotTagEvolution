public class ThymioSeeker : Thymio
{
    protected override void RobotController()
    {
        throw new System.NotImplementedException();
    }

    protected override void UpdateLEDColor()
    {

        if (leftLightSensorValue == LightSensorValue.SafeZone &&
                 rightLightSensorValue == LightSensorValue.SafeZone)
        {
            LEDRenderer.material = InSafeZoneColor;
        }
        else
        {
            LEDRenderer.material = SeekingColor;
        }
    }



}