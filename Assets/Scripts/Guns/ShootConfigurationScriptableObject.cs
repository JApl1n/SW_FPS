using UnityEngine;

// Allows us to create from unity create menu (at the top)
[CreateAssetMenu(fileName = "Shoot Config", menuName = "Guns/Shoot Configuration", order = 2)]
public class ShootConfigurationScriptableObject : ScriptableObject
{
    public bool isHitScan = true;
    public Bullet bulletPrefab;
    public float bulletSpawnForce = 1000;
    public LayerMask hitMask;
    public Vector3 spread = new Vector3(0.1f, 0.1f, 0.1f);
    public float fireRate = 0.25f;

    [SerializeField] private float spreadMultiplier = 1f;
    public float recoilRecoverySpeed = 1f;
    public float maxSpreadTime = 1f;

    private Vector3 currentSpread = Vector3.zero;


    public Vector3 GetSpread(float shootTime = 0) {
        currentSpread = Vector3.Lerp(spread, new Vector3(
            Random.Range(-spread.x * spreadMultiplier, spread.x * spreadMultiplier),
            Random.Range(-spread.y * spreadMultiplier, spread.y * spreadMultiplier),
            Random.Range(-spread.z * spreadMultiplier, spread.z * spreadMultiplier)), 
            Mathf.Clamp01(shootTime/maxSpreadTime)
        );
        
        return currentSpread;
    }
}
