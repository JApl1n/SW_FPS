using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;
using GOAP.Behaviours;
using GOAP.Config;

namespace GOAP {
    [GoapId("Objective-3259ee8e-f48b-4a13-8b2c-54c3570340d4")]
    // Action for agent to travel along NavMesh to main Objective
    public class PursueObjectiveAction : GoapActionBase<PursueObjectiveAction.Data>, IInjectable{

        private AttackConfigScriptableObject attackConfig;
        private UnityEngine.AI.NavMeshAgent nmAgent;

        public override void Start(IMonoAgent agent, Data data) {
            nmAgent = agent.GetComponent<AgentMoveBehaviour>().nmAgent;
        }

        public override IActionRunState Perform(IMonoAgent agent, Data data, IActionContext context) {
            if (nmAgent.remainingDistance > attackConfig.rangedAttackRadius)
                return ActionRunState.Continue; 
            return ActionRunState.Completed;
        }

        
        public override void Complete(IMonoAgent agent, Data data) {
            Debug.Log("Completed");
        }

        public override void Stop(IMonoAgent agent, Data data) {
            Debug.Log("Stopped");
        }


        public class Data : IActionData {
            public ITarget Target { get; set; }
        }

        public void Inject(DependencyInjector injector) {
            attackConfig = injector.attackConfig;
        }
    }
}