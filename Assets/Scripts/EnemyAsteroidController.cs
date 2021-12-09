using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAsteroidController : NetworkBehaviour
{
    GameObject targetPlayer;

    public Rigidbody bullet;
    public Transform bulletPosition;
    public Slider healthBar;

    private float travelSpeed = 5.0f;
    private GameObject[] players;
    private GameObject turret;
    private float bulletSpeed = 15.0f;
    private float rotateSpeed = 4.0f;

    private GameObject manager;
    private LevelManager levelManager;

    private NetworkVariableFloat health  = new NetworkVariableFloat(100.0f);

    // Start is called before the first frame update
    void Start()
    {        
        manager = GameObject.FindGameObjectWithTag("Level Manager");
        levelManager = manager.GetComponent<LevelManager>();

        Debug.Log("Starting Asteroid");
        InvokeRepeating("EngageTarget", 1.0f, 1.0f);

        turret = transform.Find("Turret 1a").gameObject.transform.Find("Base").gameObject;
        players = GameObject.FindGameObjectsWithTag("Player");

        gameObject.GetComponentInChildren<Slider>().transform.rotation = new Quaternion(0, 180f, 0, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        healthBar.value = health.Value;
        if(OffPLayingField()) {
            Destroy(gameObject);
        } else {
            TargetAndRotate();
            transform.Translate(new Vector3(0, 0, -travelSpeed) * Time.fixedDeltaTime, Space.World);
         }
    }

    bool OffPLayingField() {
        return transform.position.z < -6;
    }

    void EngageTarget() {    
        if (targetPlayer != null)    
            EngageServerRpc();
    }

    void TargetAndRotate() {
        targetPlayer = GetNearestTarget();
        if (targetPlayer != null) {
            Vector3 targetDirection = targetPlayer.transform.position - turret.transform.position;
            targetDirection = new Vector3(targetDirection.x, turret.transform.position.y, targetDirection.z);
            float step = rotateSpeed * Time.deltaTime;
            Vector3 newDirection = Vector3.RotateTowards(turret.transform.forward, targetDirection, step, 0.0f);
            turret.transform.rotation = Quaternion.LookRotation(newDirection);
        }
    }

    GameObject GetNearestTarget() {
        
        Vector3 pos = transform.position;
        int targetIndex = 999;

        float distance = 1000f;

        for (int i = 0; i < players.Length; i++) {
            if (!players[i].GetComponent<MPPlayerAttributes>().isDead) {
                Vector3 playerPos = players[i].transform.position;
                float playerDistance = Vector3.Distance(pos, playerPos);
                if (playerDistance < distance) {
                    distance = playerDistance;
                    targetIndex = i;
                }
            }
        }

        return targetIndex != 999 ? players[targetIndex] : null;
    }

    void OnCollisionEnter(Collision collision) {
        Debug.Log("Asteroid - collision Entered");
        if (collision.gameObject.tag == "Bullet") {
            Destroy(collision.gameObject);
            RegisterAsteroidHitServerRpc(20f, collision.gameObject.GetComponent<Bullet>().spawnerID);
        }
    }

    [ServerRpc]
    private void EngageServerRpc() {
        Vector3 fireDirection = targetPlayer.transform.position - bulletPosition.transform.position;

        gameObject.GetComponent<MPBulletSpawner>().FireServerRpc(bulletPosition.position, fireDirection.normalized);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RegisterAsteroidHitServerRpc(float damage, ulong playerId) {
        health.Value -= damage;
        if (health.Value <= 0) {
                Destroy(gameObject);
        }

        foreach(GameObject player in players) {
            if (playerId == player.GetComponent<NetworkObject>().OwnerClientId)
                player.GetComponent<MPPlayerAttributes>().score.Value += 10;
        }
    }
}
