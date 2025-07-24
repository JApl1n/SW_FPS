using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerGunSelector : MonoBehaviour
{
    [SerializeField] private GunType gun;
    [SerializeField] private Transform gunParent;
    [SerializeField] private List<GunScriptableObject> guns;
    // [SerializeField] private PlayerIK inverseKinematics;

    [Header("Runtime Filled")]
    public GunScriptableObject activeGun;

    private void Start() {
        GunScriptableObject currentGun = guns.Find(currentGun => currentGun.type == gun);

        if (currentGun == null) {
            Debug.LogError($"No GunScriptableObject found for gunType: {currentGun}");
            return;
        }

        activeGun = currentGun;
        currentGun.Spawn(gunParent, this);

        // Transform[] allChildren = gunParent.GetComponentInChildren<Transform>();
        // inverseKinematics.LeftElbowIKTarget = allChildren.FirstOrDefault(child => child.name == "LeftElbow");
        // inverseKinematics.RightElbowIKTarget = allChildren.FirstOrDefault(child => child.name == "RightElbow");
        // inverseKinematics.LeftHandIKTarget = allChildren.FirstOrDefault(child => child.name == "LeftHand");
        // inverseKinematics.RightHandIKTarget = allChildren.FirstOrDefault(child => child.name == "RightHand");
    }
}
