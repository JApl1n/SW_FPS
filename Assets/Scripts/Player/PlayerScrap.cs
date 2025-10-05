using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerScrap : MonoBehaviour {
    public int scrapValue;
    [SerializeField] private TMP_Text displayValue;
    
    void Start() {
        scrapValue = 1000;
    }

    // Update is called once per frame
    void Update() {
        displayValue.text = "" + scrapValue;
    }


    public void TransferScrap(int addValue) {
        scrapValue += addValue;
    }

}
