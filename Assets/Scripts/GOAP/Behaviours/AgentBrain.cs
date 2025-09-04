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
        [SerializeField] private GameObject[] targets;
        [SerializeField] private int numTargets;

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
            this.data.currentTarget = this.data.objective;
            this.data.currentTargetHealth = this.data.objectiveHealth;
            
            targetSensor.sphereCollider.radius = attackConfig.sensorRadius;
            targets = new GameObject[16];  // Allow for the agent to see 16 targets
            numTargets = 0;  // Gets decremented automatically in the first frame of the update
        }

        private void FixedUpdate() {

            if (numTargets > 0) {
                if (data.currentTargetHealth <= 0) {
                    // Current target dies
                    numTargets--;
                    targets = ReorderArray(targets, 0);
                } 
            }

            if (numTargets > 0) {
                this.data.currentTarget = targets[0];
                // this.data.currentTargetHealth = targets[0].GetComponentInChildren<EntityHealth>().currentHealth;
                this.provider.RequestGoal<DestroyTargetGoal>();
            } else if ((data.objectiveHealth > 0) && (this.data.currentTarget != this.data.objective)) {
                // Need a new target
                    
                // this.data.currentTarget = this.data.objective;
                // this.data.currentTargetHealth = this.data.objectiveHealth;
                this.provider.RequestGoal<MoveToObjectiveGoal>();
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
            if ((target.tag == "player") || (target.tag == "turret") || (target.tag == "objective")) {
                if (NotInList(targets, target.root.gameObject, numTargets)) {
                    targets[numTargets] = target.root.gameObject;
                    Debug.Log(target.root.gameObject);
                    numTargets++;

                    if (numTargets == 1) { // If only target
                        this.data.currentTarget = targets[0];
                        // this.data.currentTargetHealth = targets[0].GetComponentInChildren<EntityHealth>().currentHealth;
                        this.provider.RequestGoal<DestroyTargetGoal>();
                    }
                }
                
                

            }
        }

        private void TargetSensorOnTargetExit(Vector3 lastKnownPosition) {
            for (int i=0; i<targets.Length; i++) {
                float dist = Vector3.Distance(agent.transform.position, targets[i].transform.position);
                if (dist > attackConfig.sensorRadius) {
                    Debug.Log("Exited");
                    numTargets--;
                    targets = ReorderArray(targets, i);
                    // FIGURE OUT
                }
            }
        }




        private GameObject[] ReorderArray(GameObject[] oldList, int index) {
            // This might be slower than some pre-written function but its not used too much
            GameObject[] newList = new GameObject[oldList.Length];

            for (int i=0; i<index; i++) {
                newList[i] = oldList[i];
            }

            for (int i=index; i<oldList.Length-1; i++) {
                newList[i] = oldList[i+1];
            }

            return newList;
        }

        private bool NotInList(GameObject[] list, GameObject item, int index) {

            for (int i=0; i<index; i++) {
                if (list[i] == item) {
                    Debug.Log("duplicate");
                    return false;
                }
            }

            return true;

        }

    }
}
