using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CloseMenu : MonoBehaviour, IPointerClickHandler {

    [SerializeField] private Crafter crafter;
    [SerializeField] private PlayerAction playerAction;

    public void OnPointerClick(PointerEventData eventData) {
        crafter.CloseMenu();
        playerAction.UnfreezeInputs(); 
    }
}