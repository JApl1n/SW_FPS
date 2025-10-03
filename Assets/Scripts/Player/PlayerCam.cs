using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [Header("Camera Sensitivity")]
    [SerializeField] private float xSens;
    [SerializeField] private float ySens;

    [SerializeField] private Transform visuals;
    [SerializeField] private Transform orientation;

    [SerializeField] private PlayerAction playerAction;

    private float xRot;
    private float yRot;

    private bool frozenCam;

    private void Start() {
        // lock cursor and hide
        frozenCam = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update() {
        if (!frozenCam) {
            float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * ySens;
            float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * xSens;

            yRot += mouseX;
            xRot -= mouseY;
            xRot = Mathf.Clamp(xRot, -70f, 70f);

            transform.rotation = Quaternion.Euler(xRot, yRot, 0);
            orientation.rotation = Quaternion.Euler(0, yRot, 0);
            visuals.rotation = Quaternion.Euler(0, yRot, 0);
        }
    }

    public void FreezeCam() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        frozenCam = true;
    }

    public void UnfreezeCam() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        frozenCam = false;
    }
}
