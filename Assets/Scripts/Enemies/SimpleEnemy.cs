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

    private bool inRange;
    private bool hasLOS;
    

    void Start() {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        bulletPool = new ObjectPool<Bullet>(CreateBullet);
        target = GameObject.Find("Objective").transform;  // I know its bad to use but for now it works
        agent.isStopped = false;
        agent.destination = target.position;
        lastTimeFired = Time.time;
    }

    
    void Update() { 
        if (agent.remainingDistance < targetDistance) { 
            if (agent.Raycast(target.position, out _)) { 
                agent.isStopped = true; 
                TryShoot(); 
            } 
        } else {
            agent.isStopped = false; 
        }
    }


    private void TryShoot() {
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
        bullet.Spawn(shootDirection.normalized * shootConfig.bulletSpawnForce);

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
        IDamageable damageable;
        if (collider.TryGetComponent(out damageable) || collider.transform.parent.TryGetComponent(out damageable)) {
            if (collider.CompareTag("player") || collider.CompareTag("objective")) {
                damageable.TakeDamage(damageConfig.GetDamage(distanceTravelled));
            }
        }
    }

    private Bullet CreateBullet() {
        return Instantiate(shootConfig.bulletPrefab);
    }

}
