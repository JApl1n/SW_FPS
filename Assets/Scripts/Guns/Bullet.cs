using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    public Vector3 spawnLocation {
        get;
        private set;
    }

    [SerializeField] private float delayedDisableTime = 2f;

    // Collision data between bullet and other rigidbody
    public delegate void CollisionEvent(Bullet bullet, Collision collision);
    public event CollisionEvent OnCollision;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    public void Spawn(Vector3 spawnForce) {
        // Spawn bullet at current position and propel
        spawnLocation = transform.position;
        transform.forward = spawnForce.normalized;
        rb.AddForce(spawnForce);
        // If bullet doesnt hit anything, disable after chosen time (per bullet prefab)
        StartCoroutine(DelayedDisable(delayedDisableTime)); 
    }

    // As is a coroutine, can be pasued using yield until next frame (WaitForSeconds used to extend)
    private IEnumerator DelayedDisable(float time) {
        yield return new WaitForSeconds(time);
        OnCollisionEnter(null);  // Pass null if no collision
    }


    private void OnCollisionEnter(Collision collision) {
        OnCollision?.Invoke(this, collision);
    }

    private void OnDisable() {
        StopAllCoroutines();  // Just in case DelayedDisable ongoing
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        OnCollision = null;
    }

}
