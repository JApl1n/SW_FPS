using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public EntityHealth health;
    [SerializeField] private Slider enemyHealthSlider;
    [SerializeField] private Camera camera;
    
    private void Start() {
        health.OnDeath += Die;
        health.OnTakeDamage += UpdateHealthBar;
        enemyHealthSlider.maxValue = health.currentHealth;
        enemyHealthSlider.value = health.maxHealth;
    }

    private void Update() {
        enemyHealthSlider.transform.rotation = camera.transform.rotation;
    }

    private void Die(Vector3 position) {
        Debug.Log("Dead");
        Destroy(this.gameObject);
    }

    private void UpdateHealthBar(int damageTaken) {
        enemyHealthSlider.value -= damageTaken;
    }

}
