using CrashKonijn.Agent.Core;
using CrashKonijn.Agent.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace GOAP.Behaviours
{
    public class AgentShootBehaviour : MonoBehaviour
    {
        private AgentBehaviour agent;
        private ITarget currentTarget;

        [SerializeField] private DamageConfigurationScriptableObject damageConfig;
        [SerializeField] private ShootConfigurationScriptableObject shootConfig;

        private bool atTarget = false;
        private ObjectPool<Bullet> bulletPool;
        private bool inRange;
        private bool hasLOS;


        private void Awake() {
            this.agent = this.GetComponent<AgentBehaviour>();
            this.atTarget = false;
            this.bulletPool = new ObjectPool<Bullet>(CreateBullet);
        }

        private void OnEnable() {
            this.agent.Events.OnActionStart += this.OnActionStart;
            this.agent.Events.OnActionComplete += this.OnActionComplete;
            this.agent.Events.OnActionStop += this.OnActionStop;
            this.agent.Events.OnTargetInRange += this.OnTargetInRange;
            this.agent.Events.OnTargetChanged += this.OnTargetChanged;
            this.agent.Events.OnTargetNotInRange += this.TargetNotInRange;
            this.agent.Events.OnTargetLost += this.TargetLost;
        }

        private void OnDisable() {
            this.agent.Events.OnActionStart -= this.OnActionStart;
            this.agent.Events.OnActionComplete -= this.OnActionComplete;
            this.agent.Events.OnActionStop -= this.OnActionStop;
            this.agent.Events.OnTargetInRange -= this.OnTargetInRange;
            this.agent.Events.OnTargetChanged -= this.OnTargetChanged;
            this.agent.Events.OnTargetNotInRange -= this.TargetNotInRange;
            this.agent.Events.OnTargetLost -= this.TargetLost;
        }

        // Action Events
        private void OnActionStart(IAction action) {
            // Debug.Log("Action " + action + " Started");
        }

        private void OnActionComplete(IAction action) {
            // Debug.Log("Action " + action + " Completed");
        }

        private void OnActionStop(IAction action) {
            // Debug.Log("Action " + action + " Stopped");
        }
        

        // Target Events
        private void TargetLost() {
            // this.currentTarget = null;
        }

        private void OnTargetInRange(ITarget target) {
            this.atTarget = true;
            // this.transform.LookAt(target.Position);
        }

        private void OnTargetChanged(ITarget target, bool inRange) {
            this.currentTarget = target;
        }

        private void TargetNotInRange(ITarget target) {
            this.atTarget = false;
            // Debug.Log(target.Position);
        }

        public void Update() {

            if (this.agent.IsPaused)
                return;
            
            if (this.currentTarget == null)
                return;

            if (!this.atTarget)
                return;

            // Therefore at target

        }

        private void OnDrawGizmos() {
            
        }


        // Gun shooting functions \/
        public void DoProjectileShoot(Vector3 shootDirection) {
            Bullet bullet = this.bulletPool.Get();
            bullet.gameObject.SetActive(true);
            bullet.OnCollision += this.HandleBulletCollision;
            bullet.transform.position = (this.agent.transform.position + this.agent.transform.forward);
            bullet.Spawn(shootDirection.normalized * shootConfig.bulletSpawnForce);
        }

        public void HandleBulletCollision(Bullet bullet, Collision collision) {
            bullet.gameObject.SetActive(false);
            bulletPool.Release(bullet);

            if (collision != null) {
                ContactPoint contactPoint = collision.GetContact(0);

                HandleBulletImpact(Vector3.Distance(contactPoint.point, bullet.spawnLocation),
                    contactPoint.point, contactPoint.normal, contactPoint.otherCollider);
            }
        }

        public void HandleBulletImpact(float distanceTravelled, Vector3 hitLocation, Vector3 hitNormal, Collider collider) {
            IDamageable damageable;
            if (collider.TryGetComponent(out damageable) || collider.transform.parent.TryGetComponent(out damageable)) {
                if (collider.CompareTag("player") || collider.CompareTag("objective") || collider.CompareTag("turret")) {
                    damageable.TakeDamage(damageConfig.GetDamage(distanceTravelled));
                }
            }
        }

        public Bullet CreateBullet() {
            return Instantiate(shootConfig.bulletPrefab);
        }
    }
}

