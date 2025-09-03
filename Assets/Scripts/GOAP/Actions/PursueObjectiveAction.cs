using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;
using GOAP.Config;

namespace GOAP {
    [GoapId("Objective-3259ee8e-f48b-4a13-8b2c-54c3570340d4")]
    // Action for agent to travel along NavMesh to main Objective
    public class PursueObjectiveAction : GoapActionBase<PursueObjectiveAction.Data>{

        private bool active = true;

        public override void Start(IMonoAgent agent, Data data) {
            active = true;
        }

        public override IActionRunState Perform(IMonoAgent agent, Data data, IActionContext context) {
            if (active)
                return ActionRunState.Continue; 
            return ActionRunState.Stop;
        }

        
        public override void Complete(IMonoAgent agent, Data data) {
            active = false;
        }

        public override void Stop(IMonoAgent agent, Data data) {
        }


        public class Data : IActionData {
            public ITarget Target { get; set; }
        }
    }
}