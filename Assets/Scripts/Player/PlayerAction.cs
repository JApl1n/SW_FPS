using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    [Header ("Key Binds")]
    [SerializeField] private KeyCode reloadKey = KeyCode.R;

    [SerializeField] private PlayerGunSelector gunSelector;


    private void Update() {

        // Shoot input 
        if (gunSelector.activeGun != null) {
            gunSelector.activeGun.Tick(Input.GetMouseButton(0), Input.GetKey(reloadKey));
        }

    }
}
