using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthManager : MonoBehaviour
{
    public EntityHealth health;
    [SerializeField] private Slider playerHealthSlider;
    private Camera sceneCamera;
    
    private void Start() {
        health.OnDeath += Die;
        health.OnTakeDamage += UpdateHealthBar;
        playerHealthSlider.maxValue = health.currentHealth;
        playerHealthSlider.value = health.maxHealth;

        sceneCamera = GameObject.Find("Camera").GetComponent<Camera>();
    }


    private void Die(Vector3 position) {
        Debug.Log("Player Dead");
    }

    private void UpdateHealthBar(int damageTaken) {
        playerHealthSlider.value -= damageTaken;
    }

}
