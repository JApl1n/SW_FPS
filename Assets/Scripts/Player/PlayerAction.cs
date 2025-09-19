using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    [Header ("Key Binds")]
    [SerializeField] private KeyCode reloadKey = KeyCode.R;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

    [Header ("Interactable Scripts")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerGunSelector gunSelector;


    private void Update() {

        // Shoot input 
        if (gunSelector.activeGun != null) {
            gunSelector.activeGun.Tick(Input.GetMouseButton(0), Input.GetKey(reloadKey));
        }

        if (Input.GetKey(interactKey)) {
            gunSelector.PickupGun();
        }

        if (playerMovement != null) {
            playerMovement.MyInput(Input.GetKey(jumpKey), Input.GetKey(sprintKey), Input.GetKeyDown(crouchKey),
                Input.GetKeyUp(crouchKey), Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }

    }
}
