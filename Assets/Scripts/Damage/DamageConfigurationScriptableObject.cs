using UnityEngine;
using static UnityEngine.ParticleSystem;

[CreateAssetMenu(fileName = "Damage Config", menuName = "Guns/Damage Config", order = 1)]
public class DamageConfigurationScriptableObject : ScriptableObject
{
    public MinMaxCurve damageCurve; // allows various implementations of damage

    private void Reset() {
        damageCurve.mode = ParticleSystemCurveMode.Curve;
    }

    public int GetDamage(float distance = 0) {
        return Mathf.CeilToInt(damageCurve.Evaluate(distance, Random.value));
    }
}
