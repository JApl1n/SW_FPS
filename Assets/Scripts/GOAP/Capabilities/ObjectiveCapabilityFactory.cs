using GOAP.Sensors;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;
using System;

namespace GOAP.Capabilities {

    [RequireComponent(typeof(DependencyInjector))]
    public class ObjectiveCapabilityFactory : CapabilityFactoryBase {

        private DependencyInjector injector;
        
        public override ICapabilityConfig Create() {
            
            injector = GameObject.Find("GOAP").GetComponent<DependencyInjector>();
            var builder = new CapabilityBuilder("ObjectiveCapability");

            builder.AddGoal<MoveToObjectiveGoal>()
                .AddCondition<IsPursuingObjective>(Comparison.GreaterThanOrEqual, 1)
                .AddCondition<ObjectiveHealth>(Comparison.SmallerThanOrEqual, 0)
                .SetBaseCost(2);

            builder.AddAction<PursueObjectiveAction>()
                .AddEffect<IsPursuingObjective>(EffectType.Increase)
                .SetTarget<ObjectiveTarget>()
                .SetStoppingDistance(injector.attackConfig.rangedAttackRadius)
                .SetBaseCost(5);

            builder.AddTargetSensor<ObjectiveTargetSensor>()
                .SetTarget<ObjectiveTarget>();

            builder.AddWorldSensor<ObjectiveHealthSensor>()
                .SetKey<ObjectiveHealth>();
            
            return builder.Build();
        }
    }
}
