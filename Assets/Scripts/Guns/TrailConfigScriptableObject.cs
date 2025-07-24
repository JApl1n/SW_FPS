using UnityEngine;

// Allows us to create from unity create menu (at the top)
[CreateAssetMenu(fileName = "Trail Config", menuName = "Guns/Gun Trail Config", order = 4)]
public class TrailConfigScriptableObject : ScriptableObject
{
    [Header("Trail Renderer Settings")]
    public Material material;
    public AnimationCurve widthCurve;
    public float duration = 0.75f;
    public float minVertexDistance = 0.1f;
    public Gradient colour;

    public float missDistance = 100f;
    public float simulationSpeed = 100f;

}
