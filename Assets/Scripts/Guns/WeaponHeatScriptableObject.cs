using UnityEngine;
using UnityEngine.UI;

// Allows us to create from unity create menu (at the top)
[CreateAssetMenu(fileName = "Heat Config", menuName = "Guns/Weapon Heat Configuration", order = 3)]
public class WeaponHeatScriptableObject : ScriptableObject
{
    [Header ("Overheat Settings")]
    public float maxWeaponHeat = 10f;
    public float shootHeatCost = 2f;
    public float coolingDelay = 1f;
    public float coolingRate = 1f;
    public float overheatPenalty = 3f;
    public float goldZonePosition = 0.75f;  // Decimal portion along heat bar to centre gold zone
    public float goldZoneWidth = 0.2f;  // Decimal portion of heat bar the gold zone takes up
    public float goldZoneReward = 5f;  // Time reward of infinite heat for weapon

    [HideInInspector] public Slider heatSlider;
    [HideInInspector] public RectTransform heatSliderBar;
    [HideInInspector] public RectTransform goldZoneBar;

    public void HandleHeatInit(Transform parent) {
        // weaponHeat = 0f;
        // overheated = false;
        // cooling = false;
        heatSlider = parent.GetComponentInChildren<Slider>();
        heatSlider.maxValue = maxWeaponHeat;
        
        foreach (RectTransform child in parent.GetComponentsInChildren<RectTransform>()) {
            if (child.name == "Heat Slider Bar") {
                heatSliderBar = child;
            } else if (child.name == "Gold Zone") {
                goldZoneBar = child;
            }
        }
        if (heatSliderBar == null) {
            Debug.LogError("Heat Slider Bar not found! Check names of gameobject and check here.");
        }  // I can't think of another way to find the specific object im looking for so this tries to check everything is working ok
        if (goldZoneBar == null) {
            Debug.LogError("Gold Zone not found! Check names of gameobject and check here.");
        }

        goldZoneBar.anchoredPosition = new Vector2((goldZonePosition-0.5f) * heatSliderBar.rect.width, 0);
        goldZoneBar.sizeDelta = new Vector2(goldZoneWidth * heatSliderBar.rect.width, goldZoneBar.rect.height);
    }


}