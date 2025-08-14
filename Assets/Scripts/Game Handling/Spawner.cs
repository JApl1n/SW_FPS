using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject enemy;

    [SerializeField] private int numEnemies = 1;
    [SerializeField] private int rowSize = 5;
    [SerializeField] private float spacing = 2f;

    private Vector3 spawnerPos;
    private Vector3 currentPos;

    private void Start() {
        spawnerPos = this.transform.position;
        for (int i=0; i<numEnemies; i++) {
            currentPos = new Vector3 (spawnerPos.x + spacing*(i%5), spawnerPos.y, 
                spawnerPos.z + spacing*(i/5));
            Instantiate(enemy, currentPos, Quaternion.identity);
        }
    }
}
