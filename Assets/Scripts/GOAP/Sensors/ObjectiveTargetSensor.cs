using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;
using GOAP.Behaviours;

namespace GOAP.Sensors
{
    public class ObjectiveTargetSensor : LocalTargetSensorBase {
        private DataBehaviour data;

        public override void Created() { }

        public override void Update() {}

        public override ITarget Sense(IActionReceiver agent, IComponentReference references, ITarget existingTarget) {
            DataBehaviour data = references.GetCachedComponent<DataBehaviour>();
            Vector3 targetPosition = data.objective.transform.position;

            // If the existing target is a `PositionTarget`, we can reuse it and just update the position.
            if (existingTarget is PositionTarget positionTarget) {
                return positionTarget.SetPosition(targetPosition);
            }
            
            return new PositionTarget(targetPosition);
        }
    }
}
