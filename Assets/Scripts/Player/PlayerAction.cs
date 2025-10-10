using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour {
    [Header ("Key Binds")]
    [SerializeField] private KeyCode escapeKey = KeyCode.Escape;
    [SerializeField] private KeyCode reloadKey = KeyCode.R;
    public KeyCode interactKey = KeyCode.E;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

    [Header ("Interactable Scripts")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerGunSelector gunSelector;
    [SerializeField] private Crafter crafter;
    [SerializeField] private PlayerCam cameraInputScript;


    [Header ("Interactable Layer")]
    [SerializeField] private LayerMask interactableUILayer;
    [SerializeField] private Transform sceneCamera;

    public bool inputsFrozen = false;

    private string itemType;
    private string gunName;
    private GameObject interactableObject;

    private void Start() {
        inputsFrozen = false;
    }

    private void Update() {
        if (!inputsFrozen) {
            // Shoot input 
            if (gunSelector.activeGun != null) {
                gunSelector.activeGun.Tick(Input.GetMouseButton(0), Input.GetKey(reloadKey));
            }

            if (Input.GetKey(interactKey)) {
                RaycastHit hit;
                if (Physics.Raycast(sceneCamera.position, sceneCamera.forward, out hit, 10f, interactableUILayer)) {
                    if (hit.transform.root.gameObject.GetComponent<InteractableItemDisplay>() != null) {
                        interactableObject = hit.transform.root.gameObject;
                        itemType = interactableObject.GetComponent<InteractableItemDisplay>().itemType;
                        gunName = interactableObject.GetComponent<InteractableItemDisplay>().itemName;

                        if (itemType == "gun") {
                            gunSelector.PickupGun(gunName);
                            Destroy(interactableObject);
                        } else if (itemType == "crafter") {
                            crafter.Interact();
                        } else {
                            Debug.Log("itemType of " + itemType + " not valid interactable");
                        }
                    }
                }
            }

            if (playerMovement != null) {
                playerMovement.MyInput(Input.GetKey(jumpKey), Input.GetKey(sprintKey), Input.GetKeyDown(crouchKey),
                    Input.GetKeyUp(crouchKey), Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            }

        } else if (Input.GetKey(escapeKey)) {
            crafter.CloseMenu();
            UnfreezeInputs();
        }
    }

    public void FreezeInputs() {
        inputsFrozen = true;
        cameraInputScript.FreezeCam();
    }

    public void UnfreezeInputs() {
        inputsFrozen = false;
        cameraInputScript.UnfreezeCam();
    }
}
