using UnityEngine;

namespace GOAP.Config {
    [CreateAssetMenu(menuName = "AI/Attack Config", fileName = "Attack Config", order = 1)]
    public class AttackConfigScriptableObject : ScriptableObject {
        public float sensorRadius = 20;
        public float rangedAttackRadius = 10;
        public int rangedAttackCost = 1;
        public LayerMask attackableLayerMask;
    }
}
