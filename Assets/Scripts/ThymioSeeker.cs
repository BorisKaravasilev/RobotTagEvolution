using System.Collections.Generic;
using UnityEngine;

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
    
    public override void updateFitness()
    {
        double fitness = 0.0;
        foreach (Thymio robot in RobotsInFOV)
        {
            if (!robot.Tagged) {
                fitness += 10.0;
                Vector3 robotPosition = robot.transform.position;
                robotPosition.y = RaspberryPiCamera.position.y;

                Vector3 vectorToRobot = robotPosition - RaspberryPiCamera.position;

                fitness = fitness - vectorToRobot.magnitude;


            }
        }
        IncrementFitness(fitness);


    }



}