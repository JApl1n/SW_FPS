using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crafter : MonoBehaviour {
    [Header ("Scripts")]
    [SerializeField] private PlayerScrap playerScrap;
    [SerializeField] private PlayerAction playerAction;

    [Header ("Objects")]
    [SerializeField] private GameObject visuals;
    [SerializeField] private GameObject crafterUI;
    [SerializeField] private GameObject craftingMenu;

    private bool crafted;

    public void Start() {
        visuals.SetActive(false);
        craftingMenu.SetActive(false);
        crafted = false;
    }

    private void Spawn() {
        visuals.SetActive(true);
    }

    public void Interact() {
        if (!crafted) {
            if (playerScrap.scrapValue >= 0) {
                playerScrap.scrapValue -= 0;
                Spawn();
                Destroy(crafterUI);
                crafted = true;
            } else {
                Debug.Log("Not enough scrap to build!");
            }
        } else {
            playerAction.FreezeInputs();
            // Open Crafting menu
            craftingMenu.SetActive(true);
        }
    }

    public void CloseMenu() {
        //Close menu
        craftingMenu.SetActive(false);
    }
}
