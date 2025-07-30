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
    public string name;
    public GameObject modelPrefab;
    public Vector3 spawnPoint;
    public Vector3 spawnRotation;

    public DamageConfigurationScriptableObject damageConfig;
    public ShootConfigurationScriptableObject shootConfig;
    public TrailConfigScriptableObject trailConfig;

    private MonoBehaviour activeMonoBehaviour;
    private GameObject model;

    private float lastShootTime;
    private float initClickTime;
    private float stopShootTime;
    private bool lastFrameWantedToShoot;

    [Header ("Overheat Settings")]
    public float maxWeaponHeat = 10f;
    public float shootHeatCost = 2f;
    public float coolingDelay = 1f;
    public float coolingRate = 1f;
    public float overheatPenalty = 3f;
    public Slider heatSlider;

    private float weaponHeat = 0f;
    private bool overHeated = false;


    private ParticleSystem shootSystem;
    private ObjectPool<TrailRenderer> trailPool;

    public void Spawn(Transform parent, MonoBehaviour activeMonoBehaviour) {
        this.activeMonoBehaviour = activeMonoBehaviour;
        lastShootTime = 0;  // Reset because editor value used
        trailPool = new ObjectPool<TrailRenderer>(CreateTrail);

        model = Instantiate(modelPrefab);
        model.transform.SetParent(parent, false);
        model.transform.localPosition = spawnPoint;
        model.transform.localRotation = Quaternion.Euler(spawnRotation);

        weaponHeat = 0f;
        overHeated = false;
        coolingDelay = 1f;
        heatSlider = parent.GetComponentInChildren<Slider>();

        shootSystem = model.GetComponentInChildren<ParticleSystem>();
    }

    public void Shoot() {
        if ((Time.time - lastShootTime - shootConfig.fireRate > Time.deltaTime) && (!overHeated)) {
            // Let go of fire button before last frame
            // length of spray last fire
            float lastDuration = Mathf.Clamp(0, (stopShootTime - initClickTime), shootConfig.maxSpreadTime);
            float lerpTime = (shootConfig.recoilRecoverySpeed - (Time.time - stopShootTime)) / shootConfig.recoilRecoverySpeed;
            initClickTime = Time.time - Mathf.Lerp(0, lastDuration, Mathf.Clamp01(lerpTime));
        }

        if ((Time.time > shootConfig.fireRate + lastShootTime) && (!overHeated)){

            weaponHeat +=  Mathf.Clamp(shootHeatCost, 0, maxWeaponHeat-weaponHeat); // FIGURE OUT THE MATHS HERE
            if (weaponHeat >= maxWeaponHeat) {
                overHeated = true;  // Even if now overheated, can still fire this time.
                coolingDelay += overheatPenalty;  // Make cooling delay larger to penalise overheating
            } 

            lastShootTime = Time.time;
            shootSystem.Play();  // Play shooting particle system

            Vector3 spreadAmount = shootConfig.GetSpread(Time.time - initClickTime);
            model.transform.forward += model.transform.TransformDirection(spreadAmount);
            Vector3 shootDirection = model.transform.forward ;

            if (Physics.Raycast(shootSystem.transform.position, shootDirection, out RaycastHit hit, float.MaxValue, shootConfig.hitMask)) {
                activeMonoBehaviour.StartCoroutine(PlayTrail(shootSystem.transform.position, hit.point, hit));
            } else {
                activeMonoBehaviour.StartCoroutine(PlayTrail(shootSystem.transform.position, 
                    shootSystem.transform.position + (shootDirection*trailConfig.missDistance), new RaycastHit()));
            }
            
        }
    }

    public void Tick(bool wantsToShoot) {
        model.transform.localRotation = Quaternion.Lerp(
            model.transform.localRotation, Quaternion.Euler(spawnRotation), Time.deltaTime * shootConfig.recoilRecoverySpeed);

        heatSlider.value = weaponHeat;

        if ((Time.time - lastShootTime) > coolingDelay) {
            weaponHeat -= Mathf.Clamp(Time.deltaTime * coolingRate, 0, weaponHeat);
            if ((overHeated) && (weaponHeat == 0)) {
                overHeated = false;
                coolingDelay -= overheatPenalty;
            }
        }

        if (wantsToShoot) {
            lastFrameWantedToShoot = true;
            Shoot();
        } else if (!wantsToShoot && lastFrameWantedToShoot) {
            stopShootTime = Time.time;
            lastFrameWantedToShoot = false;
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
        //     SurfaceManager.Instance.HandleImpact(
        //         hit.transform.gameObject, endPoint, hit.normal, impactType, 0
        //     );
            if (hit.collider.TryGetComponent(out IDamageable damageable)) {
                damageable.TakeDamage(damageConfig.GetDamage(distance));
            }

        }

        yield return new WaitForSeconds(trailConfig.duration);
        yield return null;
        instance.emitting = false;
        instance.gameObject.SetActive(false);
        trailPool.Release(instance);
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
}
