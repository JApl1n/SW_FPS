using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

namespace GOAP.Sensors
{
    public class ObjectiveTargetSensor : LocalTargetSensorBase
    {
        public override void Created() { }

        // Is called every frame that an agent of an `AgentType` that uses this sensor needs it.
        // This can be used to 'cache' data that is used in the `Sense` method.
        // Eg look up all the trees in the scene, and then find the closest one in the Sense method.
        public override void Update() { }

        public override ITarget Sense(IActionReceiver agent, IComponentReference references, ITarget existingTarget)
        {
            Vector3 targetPosition = GameObject.Find("Objective").transform.position;

            // If the existing target is a `PositionTarget`, we can reuse it and just update the position.
            if (existingTarget is PositionTarget positionTarget) {
                return positionTarget.SetPosition(targetPosition);
            }
            
            return new PositionTarget(targetPosition);
        }
    }
}
