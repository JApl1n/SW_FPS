using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CraftableItem : MonoBehaviour, IPointerClickHandler {

    [Header ("Inputs")]
    [SerializeField] private CraftableItemData itemData;
    [SerializeField] private GameObject modelPrefab;

    [Header ("Outputs")]
    [SerializeField] private TMP_Text displayName;
    [SerializeField] private TMP_Text displayScrapCost;
    [SerializeField] private Image image;
    [SerializeField] private GameObject displayFieldNames;
    [SerializeField] private GameObject displayFieldValues;
    // [SerializeField] private Image boughtImage;

    private TMP_Text[] fieldNameTexts;
    private TMP_Text[] fieldValueTexts;

    [Header ("Player Scripts")]
    [SerializeField] private PlayerScrap playerScrap;
    [SerializeField] private PlayerGunSelector gunSelector;

    private bool bought = false;

    void Start() {
        bought = false;

        itemData = modelPrefab.GetComponent<CraftableItemData>();

        displayName.text = itemData.name;
        displayScrapCost.text = playerScrap.scrapValue.ToString() + "/" + itemData.scrapCost.ToString();
        image.sprite = itemData.prefabImage;

        fieldNameTexts = displayFieldNames.GetComponentsInChildren<TMP_Text>();
        fieldValueTexts = displayFieldValues.GetComponentsInChildren<TMP_Text>();

        for (int i=0; i<fieldNameTexts.Length; i++) {
            fieldNameTexts[i].SetText(itemData.fieldNames[i]);
            fieldValueTexts[i].SetText(itemData.fieldValues[i].ToString());
        }
    }


    public void OnPointerClick(PointerEventData eventData) {
        if (bought) {
            GivePlayerItem();
        } else if (playerScrap.scrapValue >= itemData.scrapCost) {
            playerScrap.scrapValue -= itemData.scrapCost;
            bought = true;
            displayScrapCost.text = "Owned";
            itemData.scrapCost = 0;
            GivePlayerItem();
        }
    }

    private void GivePlayerItem() {
        gunSelector.PickupGun(itemData.name);
    }
}