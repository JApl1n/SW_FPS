using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerGunSelector : MonoBehaviour
{
    [SerializeField] private GunType gun;
    [SerializeField] private Transform gunParent;
    [SerializeField] private List<GunScriptableObject> guns;

    [SerializeField] private LayerMask gunUILayer;

    [Header("Runtime Filled")]
    public GunScriptableObject activeGun;

    private string gunName;

    private void Start() {
        GunScriptableObject currentGun = guns.Find(currentGun => currentGun.type == gun);

        if (currentGun == null) {
            Debug.LogError($"No GunScriptableObject found for gunType: {currentGun}");
            return;
        }

        activeGun = currentGun;
        currentGun.Spawn(gunParent, this);
    }

    public void PickupGun(string gunName) {
        // https://discussions.unity.com/t/converting-a-string-to-an-enum/16705
        GunType parsedGunName = (GunType)System.Enum.Parse(typeof(GunType), gunName);
        GunScriptableObject currentGun = guns.Find(currentGun => currentGun.type == parsedGunName);

        if (currentGun == null) {
            Debug.LogError($"No GunScriptableObject found for gunType: {currentGun}. Have you added the guntype to the list of guns attatched to the player?");
            return;
        }

        activeGun.Despawn();

        activeGun = currentGun;
        currentGun.Spawn(gunParent, this);
    } 
    // GunScriptableObject currentGun = guns.Find(currentGun => currentGun.type == gun);
}
