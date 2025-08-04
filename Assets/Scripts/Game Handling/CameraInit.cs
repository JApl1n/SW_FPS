using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInit : MonoBehaviour
{
    [SerializeField] private Camera camera;

    [SerializeField] public ValueCarry valueCarry;
    void Start()
    {
        camera.fieldOfView = ValueCarry.FOV;
    }
}
