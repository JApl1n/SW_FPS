using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

namespace GOAP
{
    [GoapId("Objective-3259ee8e-f48b-4a13-8b2c-54c3570340d4")]
    // Action for agent to travel along NavMesh to main Objective
    // Has timer after completing action.
    public class PursueObjectiveAction : GoapActionBase<PursueObjectiveAction.Data> {
        
        public override void Start(IMonoAgent agent, Data data) {
            data.Timer = Random.Range(0.75f, 1.25f);
        }

        public override IActionRunState Perform(IMonoAgent agent, Data data, IActionContext context) {
            if (data.Timer <= 0f)
                return ActionRunState.Completed;
            
            data.Timer -= context.DeltaTime;
            
            return ActionRunState.Continue;
        }


        public class Data : IActionData {
            public ITarget Target { get; set; }
            public float Timer { get; set; }
        }
    }
}