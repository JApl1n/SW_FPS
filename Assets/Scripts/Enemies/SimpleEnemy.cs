using Unity.AI.Navigation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SimpleEnemy : MonoBehaviour
{
    public DamageConfigurationScriptableObject damageConfig;
    public ShootConfigurationScriptableObject shootConfig;
    // public TrailConfigScriptableObject trailConfig;

    [SerializeField] private Transform target;
    [SerializeField] private UnityEngine.AI.NavMeshAgent agent;

    [SerializeField] private float targetDistance = 10;

    private float lastTimeFired;
    private ObjectPool<Bullet> bulletPool;
    

    void Start() {
        bulletPool = new ObjectPool<Bullet>(CreateBullet);
        target = GameObject.Find("Objective").transform;  // I know its bad to use but for now it works
        agent.isStopped = false;
        agent.destination = target.position;
        lastTimeFired = Time.time;
    }

    
    void Update() {
        if (agent.remainingDistance < targetDistance) {
            UnityEngine.AI.NavMeshHit hit;
            if (agent.Raycast(target.position, out hit)) {
                agent.isStopped = true;
                TryShoot(hit);
            }
        }
    }

    private void TryShoot(UnityEngine.AI.NavMeshHit hit) {
        if ((Time.time - lastTimeFired) > shootConfig.fireRate) {
            lastTimeFired = Time.time;
            DoProjectileShoot((target.position - agent.transform.position));
        }
    }


    private void DoProjectileShoot(Vector3 shootDirection) {
        Bullet bullet = bulletPool.Get();
        bullet.gameObject.SetActive(true);
        bullet.OnCollision += HandleBulletCollision;
        bullet.transform.position = (agent.transform.position + agent.transform.forward);
        bullet.Spawn(shootDirection * shootConfig.bulletSpawnForce);

    }

    private void HandleBulletCollision(Bullet bullet, Collision collision) {
        bullet.gameObject.SetActive(false);
        bulletPool.Release(bullet);

        if (collision != null) {
            ContactPoint contactPoint = collision.GetContact(0);

            HandleBulletImpact(Vector3.Distance(contactPoint.point, bullet.spawnLocation),
                contactPoint.point, contactPoint.normal, contactPoint.otherCollider);
        }
    }


    private void HandleBulletImpact(float distanceTravelled, Vector3 hitLocation, Vector3 hitNormal, Collider collider) {
        if (collider.transform.parent.TryGetComponent(out IDamageable damageable)) {
            damageable.TakeDamage(damageConfig.GetDamage(distanceTravelled));
        }
    }

    private Bullet CreateBullet() {
        return Instantiate(shootConfig.bulletPrefab);
    }

}
