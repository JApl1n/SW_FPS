using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using CrashKonijn.Goap.Core;
using System;
using UnityEngine;
using GOAP.Behaviours;

namespace GOAP.Sensors {
    public class PlayerHealthSensor : LocalWorldSensorBase {
        private DataBehaviour data;
        public override void Created() { }

        public override void Update() {}

        public override SenseValue Sense(IActionReceiver agent, IComponentReference references) {
            DataBehaviour data = references.GetCachedComponent<DataBehaviour>();
            return new SenseValue(Mathf.CeilToInt(data.playerHealth));
        }
    }
}
