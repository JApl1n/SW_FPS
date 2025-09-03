using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;
using GOAP.Config;
using GOAP.Behaviours;

namespace GOAP.Sensors {
    public class CurrentTargetSensor : LocalTargetSensorBase, IInjectable {
        private AttackConfigScriptableObject attackConfig;
        private DataBehaviour data;

        Collider[] colliders = new Collider[1];

        public override void Created() { }

        public override void Update() {}

        public override ITarget Sense(IActionReceiver agent, IComponentReference references, ITarget existingTarget) {
            DataBehaviour data = references.GetCachedComponent<DataBehaviour>();
            return new TransformTarget(data.currentTarget.transform); 
            
            // if (Physics.OverlapSphereNonAlloc(agent.Transform.position, attackConfig.sensorRadius, colliders,
            //     attackConfig.attackableLayerMask) > 0) {
            //     data.currentTarget = colliders[0].transform.root.gameObject;
            //     Debug.Log("Target sensed: " + data.currentTarget);
            //     return new TransformTarget(data.currentTarget.transform);
            // } else {
            //     Debug.Log("No Target Sensed");
            //     data.currentTarget = data.objective;
            //     return new TransformTarget(data.currentTarget.transform);
            // }
            // return null;
        }

        public void Inject(DependencyInjector injector) {
            attackConfig = injector.attackConfig;
        }
    }
}
