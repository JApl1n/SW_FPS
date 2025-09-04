using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP.Behaviours {
    public class DataBehaviour : MonoBehaviour {
        // [HideInInspector] public float objectiveHealth;
        // [HideInInspector] public float playerHealth;
        // [HideInInspector] public float currentTargetHealth;

        // [HideInInspector] public GameObject objective;
        // [HideInInspector] public GameObject player;
        // [HideInInspector] public GameObject currentTarget;

        public float objectiveHealth;
        public float playerHealth;
        public float currentTargetHealth;
        
        public GameObject objective;
        public GameObject player;
        public GameObject currentTarget;


        private void Start() {
            objective = GameObject.Find("Objective");
            player = GameObject.Find("Player");
            currentTarget = objective;
            // objectiveHealth = objective.GetComponentInChildren<EntityHealth>().currentHealth;
            // playerHealth = player.GetComponentInChildren<EntityHealth>().currentHealth;
        }

        private void Update() {
            objectiveHealth = objective.GetComponentInChildren<EntityHealth>().currentHealth;
            playerHealth = player.GetComponentInChildren<EntityHealth>().currentHealth;
            if (currentTarget.GetComponentInChildren<EntityHealth>() != null) {
                currentTargetHealth = currentTarget.GetComponentInChildren<EntityHealth>().currentHealth;
            } else {
                currentTargetHealth = 0f;
            }
        }  
    }
}
