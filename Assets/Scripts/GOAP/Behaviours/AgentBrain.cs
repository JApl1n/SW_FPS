using CrashKonijn.Agent.Runtime;
using CrashKonijn.Goap.Runtime;
using UnityEngine;
using GOAP.Config;

namespace GOAP.Behaviours {
    public class BrainBehaviour : MonoBehaviour {
        private AgentBehaviour agent;
        private GoapActionProvider provider;
        private GoapBehaviour goap;

        private DataBehaviour data;

        [SerializeField] private TargetSensor targetSensor;
        [SerializeField] private AttackConfigScriptableObject attackConfig;
        
        private void Awake() {
            this.goap = FindObjectOfType<GoapBehaviour>();
            this.agent = this.GetComponent<AgentBehaviour>();
            this.provider = this.GetComponent<GoapActionProvider>();
            
            this.data = this.GetComponent<DataBehaviour>();

            if (this.provider.AgentTypeBehaviour == null)
                this.provider.AgentType = this.goap.GetAgentType("ScriptAgent");
        }

        private void Start() {
            this.provider.RequestGoal<MoveToObjectiveGoal>();
            targetSensor.sphereCollider.radius = attackConfig.sensorRadius;
        }

        private void Update() {
            if ((data.objectiveHealth > 0) && (data.currentTargetHealth <= 0)) {
                // Need a new target
                this.provider.RequestGoal<MoveToObjectiveGoal>();
                this.data.currentTarget = this.data.objective;
                this.data.currentTargetHealth = this.data.objectiveHealth;
            } 
        }

        private void OnEnable() {
            targetSensor.OnTargetEnter += TargetSensorOnTargetEnter;
            targetSensor.OnTargetExit += TargetSensorOnTargetExit;
        }

        private void OnDisable() {
            targetSensor.OnTargetEnter -= TargetSensorOnTargetEnter;
            targetSensor.OnTargetExit -= TargetSensorOnTargetExit;
        }

        private void TargetSensorOnTargetEnter(Transform target) {
            if ((target.tag == "player") || ((target.tag == "turret"))) {
                // this.agent.CurrentAction.Complete(agent, null);
                this.data.currentTarget = target.parent.gameObject;
                // Debug.Log("Target triggered " + target.parent.gameObject);
                this.data.currentTargetHealth = target.GetComponent<EntityHealth>().currentHealth;
                this.provider.RequestGoal<DestroyTargetGoal>(); 
            }
        }

        private void TargetSensorOnTargetExit(Vector3 lastKnownPosition) {
            // this.agent.CurrentAction.Complete(agent, null);
            Debug.Log("Exit");
            this.data.currentTarget = this.data.objective;
            this.data.currentTargetHealth = this.data.objectiveHealth;
            this.provider.RequestGoal<MoveToObjectiveGoal>();
        }
    }
}
