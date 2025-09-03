using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretHealthManager : MonoBehaviour {
    public EntityHealth health;
    [SerializeField] private Slider turretHealthSlider;
    private Camera sceneCamera;
    
    private void Start() {
        health.OnDeath += Die;
        health.OnTakeDamage += UpdateHealthBar;
        // turretHealthSlider.maxValue = health.currentHealth;
        // turretHealthSlider.value = health.maxHealth;
    }


    private void Die(Vector3 position) {
        Debug.Log("Turret Destroyed");
        Destroy(this.gameObject);
    }

    private void UpdateHealthBar(int damageTaken) {
        // turretHealthSlider.value -= damageTaken;
    }

}
