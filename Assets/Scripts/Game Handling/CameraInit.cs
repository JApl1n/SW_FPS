using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInit : MonoBehaviour
{
    [SerializeField] private Camera sceneCamera;

    [SerializeField] public ValueCarry valueCarry;
    void Start()
    {
        sceneCamera.fieldOfView = ValueCarry.FOV;
    }
}
