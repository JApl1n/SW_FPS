using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Waves/Wave Set", fileName = "NewWaveSet")]
public class WaveSet : ScriptableObject {
    public List<Wave> waves = new List<Wave>();
}
