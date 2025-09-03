using GOAP.Sensors;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;
using System;

namespace GOAP.Capabilities {
    
    [RequireComponent(typeof(DependencyInjector))]
    public class ShootCapabilityFactory : CapabilityFactoryBase {
        
        private DependencyInjector injector;
        
        public override ICapabilityConfig Create() {
            
            injector = GameObject.Find("GOAP").GetComponent<DependencyInjector>();
            var builder = new CapabilityBuilder("ShootCapability");

            builder.AddGoal<DestroyTargetGoal>()
                .AddCondition<TargetHealth>(Comparison.SmallerThanOrEqual, 0)
                .SetBaseCost(1);

            builder.AddAction<ShootTargetAction>()
                .AddEffect<TargetHealth>(EffectType.Decrease)
                .SetTarget<CurrentTarget>()
                .SetBaseCost(injector.attackConfig.rangedAttackCost)
                .SetStoppingDistance(injector.attackConfig.rangedAttackRadius);

            builder.AddTargetSensor<CurrentTargetSensor>()
                .SetTarget<CurrentTarget>();

            builder.AddWorldSensor<TargetHealthSensor>()
                .SetKey<TargetHealth>();
            
            return builder.Build();
        }
    }
}
