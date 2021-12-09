using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class MPBulletSpawner : NetworkBehaviour
{
    public Rigidbody bullet;
    private float bulletSpeed = 10f;

    // Update is called once per frame
    void Update()
    {
        /*if(Input.GetButtonDown("Fire1") && IsOwner) {
            FireServerRpc();
        }*/
    }

    [ServerRpc]
    public void FireServerRpc(Vector3 spawnLocation, Vector3 fireDirection, ServerRpcParams rpcParams = default) {
        Rigidbody bulletClone = Instantiate(bullet, spawnLocation, transform.rotation);
        bulletClone.GetComponent<Bullet>().spawnerID = rpcParams.Receive.SenderClientId;
        bulletClone.AddForce(fireDirection * bulletSpeed, ForceMode.Impulse);
        bulletClone.GetComponent<NetworkObject>().Spawn();
    }
}
