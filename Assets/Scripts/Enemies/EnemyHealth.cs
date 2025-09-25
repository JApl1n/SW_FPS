using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GOAP.Behaviours;

public class EnemyHealth : MonoBehaviour {
    public EntityHealth health;
    [SerializeField] private AgentBrainBehaviour agentBrain;
    [SerializeField] private Slider enemyHealthSlider;
    private Image[] images;
    private Image slider;
    private Camera sceneCamera;

    [SerializeField] GameObject scrapPrefab;
    
    private void Start() {
        health.OnDeath += Die;
        health.OnTakeDamage += UpdateHealthBar;
        enemyHealthSlider.maxValue = health.currentHealth;
        enemyHealthSlider.value = health.maxHealth;

        // These might cause problems if the position of UI elements are moved or camera's name
        // Is changed but it works for now.
        images = enemyHealthSlider.GetComponentsInChildren<Image>();
        slider = images[1];
        sceneCamera = GameObject.Find("Camera").GetComponent<Camera>();
    }

    private void Update() {
        enemyHealthSlider.transform.rotation = sceneCamera.transform.rotation;
    }

    private void Die(Vector3 position) {
        scrapPrefab.GetComponent<Scrap>().scrapValue = Random.Range(1,3);
        Instantiate(scrapPrefab, this.transform.position, Quaternion.identity);
        Destroy(health.gameObject);
        Destroy(this.gameObject);
    }

    private void UpdateHealthBar(int damageTaken, GameObject attacker) {
        agentBrain.TakeHit(damageTaken, attacker);
        enemyHealthSlider.value -= damageTaken;
        if (enemyHealthSlider.value == 0) {
            slider.color = new Color(slider.color[0], slider.color[1], slider.color[2], 0);
            // Make transparent
        }
    }
}
