using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;

public class MPPlayerController : NetworkBehaviour
{
    public float movementSpeed = 5.0f;
    public GameObject playerIndicator;    
    public Transform bulletPosition;
    
    private NetworkVariableInt playerIndex = new NetworkVariableInt(0);
    private bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsHost) {
            PlayerIndexServerRpc(1);
        }
        
        if (!IsOwner) {
            playerIndicator.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {        
        if (IsOwner && !isDead) {
            MovePlayer();

            if(Input.GetButtonDown("Fire1")) {
                gameObject.GetComponent<MPBulletSpawner>().FireServerRpc(bulletPosition.position, transform.forward);
            }
        }
    }

    void MovePlayer()
    {
        Vector3 moveVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        transform.Translate(moveVector * movementSpeed * Time.deltaTime);
    }

    [ServerRpc]
    private void PlayerIndexServerRpc(int i) {
        playerIndex.Value = i;
    }

    private void OnEnable() {
        playerIndex.OnValueChanged += PlayerIndexChange;
    }

    private void OnDisable() {
        playerIndex.OnValueChanged -= PlayerIndexChange;
    }

    private void PlayerIndexChange(int oldIndex, int newIndex) {
        if (!IsClient)
            return;
        if (newIndex == 1)
            GetComponent<MeshRenderer>().material = Resources.Load<Material>("StarSparrow_Blue");
    }
}
