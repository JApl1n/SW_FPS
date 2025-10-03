using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CraftableItem : MonoBehaviour, IPointerClickHandler {

    [Header ("Inputs")]
    [SerializeField] private string name;
    [SerializeField] private int scrapCost;
    [SerializeField] private Sprite prefabImage;
    [SerializeField] private string[] fieldNames;
    [SerializeField] private float[] fieldValues;

    [Header ("Outputs")]
    [SerializeField] private TMP_Text displayName;
    [SerializeField] private TMP_Text displayScrapCost;
    [SerializeField] private Image image;
    [SerializeField] private GameObject displayFieldNames;
    [SerializeField] private GameObject displayFieldValues;

    private TMP_Text[] fieldNameTexts;
    private TMP_Text[] fieldValueTexts;

    [Header ("Player Scripts")]
    [SerializeField] private PlayerScrap playerScrap;

    void Start() {
        displayName.text = name;
        displayScrapCost.text = scrapCost.ToString();
        image.sprite = prefabImage;

        fieldNameTexts = displayFieldNames.GetComponentsInChildren<TMP_Text>();
        fieldValueTexts = displayFieldValues.GetComponentsInChildren<TMP_Text>();

        for (int i=0; i<fieldNameTexts.Length; i++) {
            fieldNameTexts[i].SetText(fieldNames[i]);
            fieldValueTexts[i].SetText(fieldValues[i].ToString());
        }
    }

    void Update() {

    }

    public void OnPointerClick(PointerEventData eventData) {
        // Debug.Log(eventData.pointerPress);
        if (playerScrap.scrapValue >= scrapCost) {
            playerScrap.scrapValue -= scrapCost;
            Debug.Log("bought");
            // Give player item
        }
    }
}