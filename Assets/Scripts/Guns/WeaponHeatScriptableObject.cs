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
    
    // private Slider heatSlider;
    // private RectTransform heatSliderBar;


}