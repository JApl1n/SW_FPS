using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

using UnityEngine.UI;

// Allows us to create from unity create menu (at the top)
[CreateAssetMenu(fileName = "Gun", menuName = "Guns/Gun", order = 0)]
public class GunScriptableObject : ScriptableObject
{
    // public ImpactType impactType;
    public GunType type;
    // public string name;
    public GameObject modelPrefab;
    public Vector3 spawnPoint;
    public Vector3 spawnRotation;

    public DamageConfigurationScriptableObject damageConfig;
    public ShootConfigurationScriptableObject shootConfig;
    public TrailConfigScriptableObject trailConfig;
    public WeaponHeatScriptableObject weaponHeatConfig;

    private MonoBehaviour activeMonoBehaviour;
    private GameObject model;

    private float lastShootTime;
    private float initClickTime;
    private float stopShootTime;
    private bool lastFrameWantedToShoot;

    private float weaponHeat = 0f;
    private float currentCoolingDelay;
    private bool overheated = false;
    private bool cooling = false;
    const int coolingDelayMultiplier = 8;  // Used to make a coolingDelay bigger then smaller. Keep relatively small toa void holding relaod and shoot exploit


    private ParticleSystem shootSystem;
    private ObjectPool<TrailRenderer> trailPool;
    private ObjectPool<Bullet> bulletPool;
 
    public void Spawn(Transform parent, MonoBehaviour activeMonoBehaviour) {
        this.activeMonoBehaviour = activeMonoBehaviour;
        lastShootTime = 0;  // Reset because editor value used
        trailPool = new ObjectPool<TrailRenderer>(CreateTrail);
        if (!shootConfig.isHitScan) {
            bulletPool = new ObjectPool<Bullet>(CreateBullet);
        }


        model = Instantiate(modelPrefab);
        model.transform.SetParent(parent, false);
        model.transform.localPosition = spawnPoint;
        model.transform.localRotation = Quaternion.Euler(spawnRotation);

        weaponHeatConfig.HandleHeatInit(parent);
        weaponHeat = 0f; overheated = false; cooling = false; currentCoolingDelay = weaponHeatConfig.coolingDelay;

        shootSystem = model.GetComponentInChildren<ParticleSystem>();
    }

    public void Shoot() {
        if (Time.time - lastShootTime - shootConfig.fireRate > Time.deltaTime) {
            // Let go of fire button before last frame
            // length of spray last fire
            float lastDuration = Mathf.Clamp(0, (stopShootTime - initClickTime), shootConfig.maxSpreadTime);
            float lerpTime = (shootConfig.recoilRecoverySpeed - (Time.time - stopShootTime)) / shootConfig.recoilRecoverySpeed;
            initClickTime = Time.time - Mathf.Lerp(0, lastDuration, Mathf.Clamp01(lerpTime));
        }

        if (Time.time > shootConfig.fireRate + lastShootTime){

            weaponHeat +=  Mathf.Clamp(weaponHeatConfig.shootHeatCost, 0, weaponHeatConfig.maxWeaponHeat-weaponHeat); 
            if (weaponHeat >= weaponHeatConfig.maxWeaponHeat) {
                overheated = true;  // Even if now overheated, can still fire this time.
                currentCoolingDelay += weaponHeatConfig.overheatPenalty;  // Make cooling delay larger to penalise overheating
            } 

            lastShootTime = Time.time;
            shootSystem.Play();  // Play shooting particle system

            Vector3 spreadAmount = shootConfig.GetSpread(Time.time - initClickTime);
            model.transform.forward += model.transform.TransformDirection(spreadAmount);
            Vector3 shootDirection = model.transform.forward ;

            if (shootConfig.isHitScan) {
                DoHitscanShoot(shootDirection);
            } else {
                DoProjectileShoot(shootDirection);
            }

            
            
        }
    }

    private void DoHitscanShoot(Vector3 shootDirection) {
        if (Physics.Raycast(shootSystem.transform.position, shootDirection, out RaycastHit hit, float.MaxValue, shootConfig.hitMask)) {
                activeMonoBehaviour.StartCoroutine(PlayTrail(shootSystem.transform.position, hit.point, hit));
            } else {
                activeMonoBehaviour.StartCoroutine(PlayTrail(shootSystem.transform.position, 
                    shootSystem.transform.position + (shootDirection*trailConfig.missDistance), new RaycastHit()));
            }
    }


    private void DoProjectileShoot(Vector3 shootDirection) {
        Bullet bullet = bulletPool.Get();
        bullet.gameObject.SetActive(true);
        bullet.OnCollision += HandleBulletCollision;
        bullet.transform.position = shootSystem.transform.position + (shootSystem.transform.forward*shootConfig.bulletSpawnOffset);
        bullet.Spawn(shootDirection * shootConfig.bulletSpawnForce);

        // TrailRenderer trail = trailPool.Get();
        // if (trail != null) {
        //     trail.transform.SetParent(bullet.transform, false);
        //     trail.transform.localPosition = Vector3.zero;
        //     trail.emitting = true;
        //     trail.gameObject.SetActive(true);
        // }

    }


    private void HandleBulletCollision(Bullet bullet, Collision collision) {
        // TrailRenderer trail = bullet.GetComponentInChildren<TrailRenderer>();
        // if (trail != null) {
        //     trail.transform.SetParent(null, true);
        //     activeMonoBehaviour.StartCoroutine(DelayedDisableTrail(trail));
        // }

        bullet.gameObject.SetActive(false);
        bulletPool.Release(bullet);

        if (collision != null) {
            ContactPoint contactPoint = collision.GetContact(0);

            HandleBulletImpact(Vector3.Distance(contactPoint.point, bullet.spawnLocation),
                contactPoint.point, contactPoint.normal, contactPoint.otherCollider);
        }
    }


    private void HandleBulletImpact(float distanceTravelled, Vector3 hitLocation, Vector3 hitNormal, Collider collider) {
        if (collider.TryGetComponent(out IDamageable damageable)) {
            if (collider.CompareTag("enemy")) {
                damageable.TakeDamage(damageConfig.GetDamage(distanceTravelled));
            }
        }
    }


    public void Tick(bool wantsToShoot, bool wantsToCool) {
        model.transform.localRotation = Quaternion.Lerp(
            model.transform.localRotation, Quaternion.Euler(spawnRotation), Time.deltaTime * shootConfig.recoilRecoverySpeed);

        // Update heat slider UI
        // Look into changing colour with heat
        weaponHeatConfig.heatSlider.value = weaponHeat;
        if (weaponHeat == 0) {
                weaponHeatConfig.heatSliderBarImage.color = new Color(0,0,0,0); 
            // Make transparent
        } else {
            weaponHeatConfig.heatSliderBarImage.color = weaponHeatConfig.barColour;
        }

        if (wantsToCool && (!overheated) && (!cooling)) {
            if ((weaponHeat>=(weaponHeatConfig.maxWeaponHeat*(weaponHeatConfig.goldZonePosition-weaponHeatConfig.goldZoneWidth/2)) && 
                (weaponHeat<=(weaponHeatConfig.maxWeaponHeat*(weaponHeatConfig.goldZonePosition+weaponHeatConfig.goldZoneWidth/2))))) {
                // Weapon cooled in gold zone
                weaponHeat = 0f;
            } else {
                // Weapon cooled outside of gold zone
                cooling = true;
                currentCoolingDelay /= coolingDelayMultiplier;
            }
        }

        if ((Time.time - lastShootTime) > currentCoolingDelay) {
            weaponHeat -= Mathf.Clamp(Time.deltaTime * weaponHeatConfig.coolingRate, 0, weaponHeat);

            if ((overheated) && (weaponHeat == 0)) {
                overheated = false;
                currentCoolingDelay -= weaponHeatConfig.overheatPenalty;
            } else if (cooling && (weaponHeat == 0)) {
                cooling = false;
                currentCoolingDelay *= coolingDelayMultiplier;
            }
        }

        if (!overheated && ! cooling) {
            if (wantsToShoot) {
                lastFrameWantedToShoot = true;
                Shoot();
            } else if (!wantsToShoot && lastFrameWantedToShoot) {
                stopShootTime = Time.time;
                lastFrameWantedToShoot = false;
            }
        }
    }

    private IEnumerator PlayTrail(Vector3 startPoint, Vector3 endPoint, RaycastHit hit) {
        TrailRenderer instance = trailPool.Get();
        instance.gameObject.SetActive(true);
        instance.transform.position = startPoint;
        // yield return null;  // make sure using correct frame (in case previous frame position used)

        instance.emitting = true;

        float distance = Vector3.Distance(startPoint, endPoint);
        float remainingDistance = distance;

        while (remainingDistance > 0) {
            instance.transform.position = Vector3.Lerp(startPoint, endPoint, Mathf.Clamp01(1 - (remainingDistance/distance)));
            remainingDistance -= trailConfig.simulationSpeed * Time.deltaTime;

            yield return null;
        }

        instance.transform.position = endPoint;

        if (hit.collider != null) {
        
            HandleBulletImpact(distance, endPoint, hit.normal, hit.collider);

        }

        yield return new WaitForSeconds(trailConfig.duration);
        yield return null;
        instance.emitting = false;
        instance.gameObject.SetActive(false);
        trailPool.Release(instance);
    }

    private IEnumerator DelayedDisableTrail(TrailRenderer trail) {
        yield return new WaitForSeconds(trailConfig.duration);
        yield return null;
        trail.emitting = false;
        trail.gameObject.SetActive(false);
        trailPool.Release(trail);
    }


    private TrailRenderer CreateTrail() {
        GameObject instance = new GameObject("Bullet Trail");
        TrailRenderer trail = instance.AddComponent<TrailRenderer>();
        trail.colorGradient = trailConfig.colour;
        trail.material = trailConfig.material;
        trail.widthCurve = trailConfig.widthCurve;
        trail.time = trailConfig.duration;
        trail.minVertexDistance = trailConfig.minVertexDistance;

        trail.emitting = false;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        return trail;
    }

    private Bullet CreateBullet() {
        return Instantiate(shootConfig.bulletPrefab);
    }   
}
