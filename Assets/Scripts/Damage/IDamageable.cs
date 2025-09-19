using UnityEngine;

public interface IDamageable {
    public int currentHealth { get; }
    public int maxHealth { get; }

    public delegate void TakeDamageEvent(int damage, GameObject attacker);
    public event TakeDamageEvent OnTakeDamage;

    public delegate void DeathEvent(Vector3 position);
    public event DeathEvent OnDeath;

    public void TakeDamage(int damage, GameObject attacker);
}
