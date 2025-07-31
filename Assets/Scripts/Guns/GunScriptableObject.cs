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
    public WeaponHeatScriptableObject weaponHeatConfig;

    private MonoBehaviour activeMonoBehaviour;
    private GameObject model;

    private float lastShootTime;
    private float initClickTime;
    private float stopShootTime;
    private bool lastFrameWantedToShoot;

    
    private Slider heatSlider;
    private RectTransform heatSliderBar;
    private RectTransform goldZoneBar;

    private float weaponHeat = 0f;
    private bool overheated = false;
    private bool cooling = false;
    const int coolingDelayMultiplier = 1024;  // Used to make a coolingDelay bigger then smaller (sets to near 0, then back to original value)


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

        HandleHeatInit(parent);
        

        shootSystem = model.GetComponentInChildren<ParticleSystem>();
    }

    public void Shoot() {
        if ((Time.time - lastShootTime - shootConfig.fireRate > Time.deltaTime) && (!overheated)) {
            // Let go of fire button before last frame
            // length of spray last fire
            float lastDuration = Mathf.Clamp(0, (stopShootTime - initClickTime), shootConfig.maxSpreadTime);
            float lerpTime = (shootConfig.recoilRecoverySpeed - (Time.time - stopShootTime)) / shootConfig.recoilRecoverySpeed;
            initClickTime = Time.time - Mathf.Lerp(0, lastDuration, Mathf.Clamp01(lerpTime));
        }

        if ((Time.time > shootConfig.fireRate + lastShootTime) && (!overheated) && (!cooling)){

            weaponHeat +=  Mathf.Clamp(weaponHeatConfig.shootHeatCost, 0, weaponHeatConfig.maxWeaponHeat-weaponHeat); 
            if (weaponHeat >= weaponHeatConfig.maxWeaponHeat) {
                overheated = true;  // Even if now overheated, can still fire this time.
                weaponHeatConfig.coolingDelay += weaponHeatConfig.overheatPenalty;  // Make cooling delay larger to penalise overheating
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

    public void Tick(bool wantsToShoot, bool wantsToCool) {
        model.transform.localRotation = Quaternion.Lerp(
            model.transform.localRotation, Quaternion.Euler(spawnRotation), Time.deltaTime * shootConfig.recoilRecoverySpeed);

        heatSlider.value = weaponHeat;

        if (wantsToCool && (!overheated) && (!cooling)) {
            if ((weaponHeat>=(weaponHeatConfig.maxWeaponHeat*(weaponHeatConfig.goldZonePosition-weaponHeatConfig.goldZoneWidth/2)) && 
                (weaponHeat<=(weaponHeatConfig.maxWeaponHeat*(weaponHeatConfig.goldZonePosition+weaponHeatConfig.goldZoneWidth/2))))) {
                // Weapon cooled in gold zone
                weaponHeat = 0f;
                Debug.Log("Cooled");
            } else {
                Debug.Log("Less cooled");
                cooling = true;
                weaponHeatConfig.coolingDelay /= coolingDelayMultiplier;
            }
        }

        if ((Time.time - lastShootTime) > weaponHeatConfig.coolingDelay) {
            weaponHeat -= Mathf.Clamp(Time.deltaTime * weaponHeatConfig.coolingRate, 0, weaponHeat);

            if ((overheated) && (weaponHeat == 0)) {
                overheated = false;
                weaponHeatConfig.coolingDelay -= weaponHeatConfig.overheatPenalty;
            } else if (cooling && (weaponHeat == 0)) {
                cooling = false;
                weaponHeatConfig.coolingDelay *= coolingDelayMultiplier;
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

    private void HandleHeatInit(Transform parent) {
        weaponHeat = 0f;
        overheated = false;
        cooling = false;
        heatSlider = parent.GetComponentInChildren<Slider>();
        heatSlider.maxValue = weaponHeatConfig.maxWeaponHeat;
        
        foreach (RectTransform child in parent.GetComponentsInChildren<RectTransform>()) {
            if (child.name == "Heat Slider Bar") {
                heatSliderBar = child;
            } else if (child.name == "Gold Zone") {
                goldZoneBar = child;
            }
        }
        if (heatSliderBar == null) {
            Debug.LogError("Heat Slider Bar not found! Check names of gameobject and check here.");
        }  // I can't think of another way to find the specific object im looking for so this tries to check everything is working ok
        if (goldZoneBar == null) {
            Debug.LogError("Gold Zone not found! Check names of gameobject and check here.");
        }

        goldZoneBar.anchoredPosition = new Vector2((weaponHeatConfig.goldZonePosition-0.5f) * heatSliderBar.rect.width, 0);
        goldZoneBar.sizeDelta = new Vector2(weaponHeatConfig.goldZoneWidth * heatSliderBar.rect.width, goldZoneBar.rect.height);
    }
}
