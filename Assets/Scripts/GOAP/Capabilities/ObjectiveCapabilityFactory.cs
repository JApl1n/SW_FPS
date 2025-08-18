using GOAP.Sensors;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;

namespace GOAP.Capabilities
{
    public class ObjectiveCapabilityFactory : CapabilityFactoryBase
    {
        public override ICapabilityConfig Create()
        {
            var builder = new CapabilityBuilder("ObjectiveCapability");

            builder.AddGoal<ObjectiveGoal>()
                .AddCondition<IsPursuingObjective>(Comparison.GreaterThanOrEqual, 1)
                .SetBaseCost(2);

            builder.AddAction<PursueObjectiveAction>()
                .AddEffect<IsPursuingObjective>(EffectType.Increase)
                .SetTarget<ObjectiveTarget>()
                .SetBaseCost(5)
                .SetInRange(10);

            builder.AddAction<ShootTargetAction>()
                .AddEffect<IsShootingTarget>(EffectType.Increase)
                .SetTarget<ObjectiveTarget>()
                .SetBaseCost(2);

            builder.AddTargetSensor<ObjectiveTargetSensor>()
                .SetTarget<ObjectiveTarget>();
            
            return builder.Build();
        }
    }
}
