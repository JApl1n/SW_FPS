using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    static float FOV = 90;

    [SerializeField] private int minFOV = 45;
    [SerializeField] private int maxFOV = 145;
    [SerializeField] private Slider FOVSlider;
    [SerializeField] private TextMeshProUGUI FOVText;

    [SerializeField] public ValueCarry valueCarry;
    
    private void Start() {
        FOVSlider.value = 90;  // Give it a good starting value
        FOVSlider.minValue = minFOV;
        FOVSlider.maxValue = maxFOV;
        FOVText.text = FOVSlider.value.ToString();
    }

    public void UpdateValue()
    {
        FOV = FOVSlider.value;
        FOVText.text = FOV.ToString();
        ValueCarry.FOV = FOV;
    }
}
