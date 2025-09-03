using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class TargetSensor : MonoBehaviour {
    public SphereCollider sphereCollider;

    public delegate void TargetEnterEvent(Transform target);

    public delegate void TargetExitEvent(Vector3 lastKnownPosition);

    public event TargetEnterEvent OnTargetEnter;

    public event TargetExitEvent OnTargetExit;

    private void Awake() {
        sphereCollider = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other) {
        if ((other != null) && (other.transform.parent != null)) {
            if (other.CompareTag("player") || other.CompareTag("objective") || other.CompareTag("turret")) {
                OnTargetEnter?.Invoke(other.transform);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if ((other != null) && (other.transform.parent != null)) {
            if (other.CompareTag("player") || other.CompareTag("objective") || other.CompareTag("turret")) {
                OnTargetExit?.Invoke(other.transform.position);
            }
        }
    }
}
