using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.SceneManagement;
using UnityEngine;

public class LevelManager : NetworkBehaviour
{
    public GameObject[] enemies;
    public GameObject[] pickups;

    public GameObject[] playerSpawns;
    public GameObject chatUI;
    public GameObject gameOverUI;

    public float minTimeBetweenSpawns;
    public float maxTimeBetweenSpawns;

    public float minXPosition;
    public float maxXPosition;

    public float zPosition;
    public float yPosition;

    private float spawnTimer = 5f;
    
    void Start() {
        InvokeRepeating("SpawnSomethingServerRpc", 1f, spawnTimer);
    }

    void Update() {
        
    }

    [ServerRpc]
    void SpawnSomethingServerRpc() {
        float chance = Random.Range(0.0f, 10.0f);
        Vector3 objectSpawn = new Vector3(Random.Range(minXPosition, maxXPosition), yPosition, zPosition);
        GameObject obj;

        if (chance <= 9 ) {
            int enemyIndex = Random.Range(0, enemies.Length);
            GameObject enemyType = enemies[enemyIndex];
            enemyType.transform.Rotate(new Vector3(0, Random.Range(0, 360), 0), Space.World);
            obj = Instantiate(enemyType, objectSpawn, enemyType.transform.rotation);
        } else {
            int pickupIndex = Random.Range(0, pickups.Length);
            GameObject pickupType = pickups[pickupIndex];
            obj = Instantiate(pickupType, objectSpawn, pickupType.transform.rotation);
        }
       
        obj.GetComponent<NetworkObject>().Spawn();
        CancelInvoke("SpawnSomethingServerRpc");
        spawnTimer = Random.Range(minTimeBetweenSpawns, maxTimeBetweenSpawns);
        InvokeRepeating("SpawnSomethingServerRpc", spawnTimer, spawnTimer);
    }

    public void CheckGameOver() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        bool allPlayersDead = true;

        if (players.Length > 0) {
            for (int i = 0; i < players.Length; i++) {
                if (!players[i].GetComponent<MPPlayerAttributes>().isDead)
                    allPlayersDead = false;
            }
        }
        
        if (allPlayersDead) {
            chatUI.SetActive(false);
            gameOverUI.SetActive(true);
        }
    }

    public void LevelExitButtonClicked() {
        NetworkSceneManager.SwitchScene("MainMenu");
    }
}
