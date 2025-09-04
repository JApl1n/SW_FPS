using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;
using GOAP.Config;
using GOAP.Behaviours;

namespace GOAP {
    [GoapId("ShootTarget-9a3df751-3643-4c08-99ac-86ad2793d9f0")]
    public class ShootTargetAction : GoapActionBase<ShootTargetAction.Data>, IInjectable {
        private AttackConfigScriptableObject attackConfig;
        private ShootConfigurationScriptableObject shootConfig;

        private float targetHealth;
        private DataBehaviour dataBehaviour;
        private AgentShootBehaviour shootBehaviour;

        public override void Start(IMonoAgent agent, Data data) {
            data.Time = 0f;
            data.LastTimeFired = data.Time;
            dataBehaviour = agent.GetComponent<DataBehaviour>();
            shootBehaviour = agent.GetComponent<AgentShootBehaviour>();
        }

        public override IActionRunState Perform(IMonoAgent agent, Data data, IActionContext context) { 
            
            // Update time
            data.Time += context.DeltaTime;

            bool shouldAttack = (data.Target != null) && (Vector3.Distance(data.Target.Position, agent.transform.position) <=
                attackConfig.rangedAttackRadius);

            // Decide attack
            if (shouldAttack) {
                if ((data.Time - data.LastTimeFired) > shootConfig.fireRate) {
                    // Perform attack
                    shootBehaviour.DoProjectileShoot((data.Target.Position - agent.transform.position));
                    data.LastTimeFired = data.Time;
                }
            }

            // Complete action if target detroyed/out of health
            targetHealth = dataBehaviour.currentTargetHealth;
            if (targetHealth == null || targetHealth <= 0) {
                return ActionRunState.Completed;
            }

            return ActionRunState.Continue;

        }

        public override void Complete(IMonoAgent agent, Data data) { }

        
        public class Data : IActionData {
            public ITarget Target { get; set; }
            public float Time { get; set; }
            public float LastTimeFired { get; set; }
        }

        public void Inject(DependencyInjector injector) {
            attackConfig = injector.attackConfig;
            shootConfig = injector.shootConfig;
        }
    }
}