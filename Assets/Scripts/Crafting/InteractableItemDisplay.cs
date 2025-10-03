using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractableItemDisplay : MonoBehaviour {

    [SerializeField] private GameObject textCanvas;
    private TMP_Text[] text;
    private Camera sceneCamera;

    private float distance;
    [SerializeField] [Range(0,4)] float minInteractDistance = 1f;
    [SerializeField] [Range(0,4)] float maxInteractDistance = 2f;

    public string itemType;
    public string itemName;

    [SerializeField] private string extraText;
    [SerializeField] private PlayerAction playerInputScript;


    private void Start() {
        sceneCamera = GameObject.Find("Camera").GetComponent<Camera>();
        text = textCanvas.GetComponentsInChildren<TMP_Text>();

        text[0].SetText(itemName);
        text[1].SetText("Press [" + playerInputScript.interactKey + "] to interact");
        text[2].SetText(extraText);

        textCanvas.SetActive(false);
    }

    private void Update() {
        distance = Vector3.Distance(this.transform.position, sceneCamera.transform.position);
        if (textCanvas != null) {
            if ((distance < maxInteractDistance) && (distance > minInteractDistance)) {
                textCanvas.SetActive(true);
                textCanvas.transform.rotation = sceneCamera.transform.rotation;
            } else {
                textCanvas.SetActive(false);
            }
        }
    }
}
