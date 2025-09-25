using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrap : MonoBehaviour {
    
    private Collider collider;

    public int scrapValue;


    void Awake() {
        collider = this.GetComponentInChildren<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other) {
        if ((other != null) && (other.transform.parent != null)) {
            if (other.CompareTag("player")) {
               other.transform.root.gameObject.GetComponent<PlayerScrap>().TransferScrap(scrapValue);
                Destroy(this.gameObject);
            }
        }
    }
}
