using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveHealthManager : MonoBehaviour
{
    public EntityHealth health;
    [SerializeField] private Slider objectiveHealthSlider;
    
    private void Start() {
        health.OnDeath += Die;
        health.OnTakeDamage += UpdateHealthBar;
        objectiveHealthSlider.maxValue = health.currentHealth;
        objectiveHealthSlider.value = health.maxHealth;
    }

    private void Die(Vector3 position) {
        Debug.Log("Game Over, Objective was destroyed");
    }

    private void UpdateHealthBar(int damageTaken) {
        objectiveHealthSlider.value -= damageTaken;
    }
}
