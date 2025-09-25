using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScrap : MonoBehaviour {
    public int scrapValue;
    
    void Start() {
        scrapValue = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void TransferScrap(int addValue) {
        scrapValue += addValue;
    }

}
