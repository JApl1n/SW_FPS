using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemies;

    [SerializeField] private int rowSize = 5;
    [SerializeField] private float spacing = 2f;

    private Vector3 spawnerPos;
    private Vector3 currentPos;

    [Header("Waves")]
    [SerializeField] private WaveSet waveSet;

    private void Start() {
        spawnerPos = this.transform.position;
        SpawnWave(waveSet, 0);
    }

    

    public void SpawnWave(WaveSet waveSet, int waveIndex) {
        var wave = waveSet.waves[waveIndex];

        foreach (var enemy in wave.enemies) {
            for (int i = 0; i < enemy.count; i++) {
                currentPos = new Vector3 (spawnerPos.x + spacing*(i%4), spawnerPos.y, 
                    spawnerPos.z + spacing*(i/rowSize));
                Instantiate(enemy.enemyPrefab, currentPos, Quaternion.identity);
            }
        }
    }
}
