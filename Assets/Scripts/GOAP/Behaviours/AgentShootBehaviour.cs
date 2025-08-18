using CrashKonijn.Agent.Core;
using CrashKonijn.Agent.Runtime;
using UnityEngine;

namespace GOAP.Behaviours
{
    public class AgentShootBehaviour : MonoBehaviour
    {
        private AgentBehaviour agent;
        private ITarget currentTarget;

        // private UnityEngine.AI.NavMeshAgent nmAgent;
        // [SerializeField] private float targetDistance = 10f;

        private void Awake()
        {
            this.agent = this.GetComponent<AgentBehaviour>();
            // this.nmAgent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
            // this.nmAgent.isStopped = false;

        }

        private void OnEnable()
        {
            this.agent.Events.OnTargetInRange += this.OnTargetInRange;
            this.agent.Events.OnTargetChanged += this.OnTargetChanged;
            this.agent.Events.OnTargetNotInRange += this.TargetNotInRange;
            this.agent.Events.OnTargetLost += this.TargetLost;
        }

        private void OnDisable()
        {
            this.agent.Events.OnTargetInRange -= this.OnTargetInRange;
            this.agent.Events.OnTargetChanged -= this.OnTargetChanged;
            this.agent.Events.OnTargetNotInRange -= this.TargetNotInRange;
            this.agent.Events.OnTargetLost -= this.TargetLost;
        }
        
        private void TargetLost()
        {
            
        }

        private void OnTargetInRange(ITarget target)
        {
            
        }

        private void OnTargetChanged(ITarget target, bool inRange)
        {
            
        }

        private void TargetNotInRange(ITarget target)
        {
            
        }

        public void Update()
        {
          
        }

        private void OnDrawGizmos()
        {
            
        }
    }
}
