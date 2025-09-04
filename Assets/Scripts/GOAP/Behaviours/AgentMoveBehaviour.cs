using CrashKonijn.Agent.Core;
using CrashKonijn.Agent.Runtime;
using UnityEngine;

namespace GOAP.Behaviours {
    public class AgentMoveBehaviour : MonoBehaviour {
        private AgentBehaviour agent;
        private ITarget currentTarget;
        private bool shouldMove;

        public UnityEngine.AI.NavMeshAgent nmAgent;

        private void Awake() {
            this.agent = this.GetComponent<AgentBehaviour>();
            this.nmAgent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
        }

        private void OnEnable() {
            this.agent.Events.OnActionStart += this.OnActionStart;
            this.agent.Events.OnActionStop += this.OnActionStop;
            this.agent.Events.OnActionComplete += this.OnActionComplete;
            this.agent.Events.OnTargetInRange += this.OnTargetInRange;
            this.agent.Events.OnTargetChanged += this.OnTargetChanged;
            this.agent.Events.OnTargetNotInRange += this.TargetNotInRange;
            this.agent.Events.OnTargetLost += this.TargetLost;
        }

        private void OnDisable() {
            this.agent.Events.OnActionStart -= this.OnActionStart;
            this.agent.Events.OnActionComplete -= this.OnActionComplete;
            this.agent.Events.OnTargetInRange -= this.OnTargetInRange;
            this.agent.Events.OnTargetChanged -= this.OnTargetChanged;
            this.agent.Events.OnTargetNotInRange -= this.TargetNotInRange;
            this.agent.Events.OnTargetLost -= this.TargetLost;
        }
        
        // Action Events
        private void OnActionStart(IAction action) {
            this.nmAgent.isStopped = true;
            this.shouldMove = true;
        }

        private void OnActionStop(IAction action) {
            this.nmAgent.isStopped = true;
            this.shouldMove = false;
        }

        private void OnActionComplete(IAction action) {
            this.nmAgent.isStopped = true;
            this.shouldMove = false;
        }


        // Target Events
        private void TargetLost() {
            this.currentTarget = null;
        }

        private void OnTargetInRange(ITarget target) {
            this.shouldMove = false;
            this.nmAgent.isStopped = true;
        }

        private void OnTargetChanged(ITarget target, bool inRange) {
            this.currentTarget = target;
            this.shouldMove = !inRange;
            this.nmAgent.destination = this.currentTarget.Position;
        }

        private void TargetNotInRange(ITarget target) {
            this.shouldMove = true;
            this.nmAgent.isStopped = false;
        }

        public void Update() {
          if (this.agent.IsPaused)
                return;

            if (!this.shouldMove)
                return;
            
            if (this.currentTarget == null)
                return;

        }

        private void OnDrawGizmos() {
            if (this.currentTarget == null)
                return;
            Gizmos.DrawLine(this.transform.position, this.currentTarget.Position);
        }
    }
}
