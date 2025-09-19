using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickup : MonoBehaviour {

    [SerializeField] private GameObject textCanvas;
    private Camera sceneCamera;

    private float distance;
    [SerializeField] [Range(1,10)] float pickupDistance = 2f;

    public string itemName;


    private void Start() {
        sceneCamera = GameObject.Find("Camera").GetComponent<Camera>();
        textCanvas.SetActive(false);
    }

    private void Update() {
        distance = Vector3.Distance(this.transform.position, sceneCamera.transform.position);
        if (distance < pickupDistance) {
            textCanvas.SetActive(true);
        } else {
            textCanvas.SetActive(false);
        }
        textCanvas.transform.rotation = sceneCamera.transform.rotation;
    }
}
