using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour {
    
    [SerializeField] private float lifetime = 1f;

    private float time = 0f;

    void Start() {
        time = 0f;
    }

    void Update() {
        time += Time.deltaTime;
        if (time > lifetime) {
            Destroy(this.gameObject);
            Debug.Log(time);
        }
    }
}
