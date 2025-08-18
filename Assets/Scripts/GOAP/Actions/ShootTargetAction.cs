using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

namespace GOAP
{
    [GoapId("ShootTarget-9a3df751-3643-4c08-99ac-86ad2793d9f0")]
    public class ShootTargetAction : GoapActionBase<ShootTargetAction.Data> {
        
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