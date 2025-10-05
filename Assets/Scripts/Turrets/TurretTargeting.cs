using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class TurretTargeting : MonoBehaviour {

    [Header ("Scripts")]
    [SerializeField] private DamageConfigurationScriptableObject damageConfig;
    [SerializeField] private ShootConfigurationScriptableObject shootConfig;
    
    [Header ("Sensing")]
    [SerializeField] private SphereCollider sphereCollider;
    [SerializeField] private float sphereRadius;
    [SerializeField] private float rotationSpeed;

    private ObjectPool<Bullet> bulletPool;

    [SerializeField] private GameObject[] enemies = new GameObject[32];
    private int numTargets;


    private float time;
    private float lastTimeFired;
    private bool shouldShoot;

    private void Awake() {
        bulletPool = new ObjectPool<Bullet>(CreateBullet);
        sphereCollider.radius = sphereRadius;
        numTargets = 0;
        time = 0f;
        lastTimeFired = 0f;
    }

    private void Update() {
        time += Time.deltaTime;

        if (numTargets > 0) {
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, enemies[numTargets-1].transform.rotation, Time.deltaTime * rotationSpeed);

            bool shouldShoot = ((time - lastTimeFired) > shootConfig.fireRate);
        
            if (shouldShoot) {
                DoProjectileShoot(-this.transform.forward);
                lastTimeFired = time;
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.transform.parent != null) {
            if (other.CompareTag("enemy")) {
                if (!InList(enemies, other.transform.root.gameObject, numTargets)) {
                    enemies[numTargets] = other.transform.root.gameObject;
                    numTargets++;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if ((other.transform.parent != null) && (other.CompareTag("enemy"))) {
            for (int i=0; i<numTargets; i++) {
                float dist = Vector3.Distance(this.transform.position, enemies[i].transform.position);
                if (dist > sphereRadius) {
                    numTargets--;
                    enemies = ReorderArray(enemies, i);
                }
            }
        }
    }



    // Gun shooting functions \/
    public void DoProjectileShoot(Vector3 shootDirection) {
        Bullet bullet = this.bulletPool.Get();
        bullet.gameObject.SetActive(true);
        bullet.OnCollision += this.HandleBulletCollision;
        bullet.transform.position = (this.transform.position + this.transform.forward*shootConfig.bulletSpawnOffset);
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
            if (collider.CompareTag("enemy")) {
                damageable.TakeDamage(damageConfig.GetDamage(distanceTravelled), this.transform.gameObject);
                // if (damageable.currentHealth == 0) {
                //     numTargets--;
                //     enemies = ReorderArray(enemies, 0);
                // }
            }
        }
    }

    public Bullet CreateBullet() {
        return Instantiate(shootConfig.bulletPrefab);
    }



    // This might be slower than some pre-written function but its not used too much
    // Simply moves upper section of list down by one according to index value given
    private GameObject[] ReorderArray(GameObject[] oldList, int index) {
        
        GameObject[] newList = new GameObject[oldList.Length];

        for (int i=0; i<index; i++) {
            newList[i] = oldList[i];
        }

        for (int i=index; i<oldList.Length-1; i++) {
            newList[i] = oldList[i+1];
        }

        return newList;
    }

    // Searches through list for item
    private bool InList(GameObject[] list, GameObject item, int index) {
        for (int i=0; i<index; i++) {
            if (list[i] == item) {
                return true;
            }
        }
        return false;
    }
}
