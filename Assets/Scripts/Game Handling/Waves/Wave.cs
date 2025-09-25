using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wave {
    [Tooltip("Enemies to spawn in this wave")]
    public List<EnemySpawnInfo> enemies = new List<EnemySpawnInfo>();
}
