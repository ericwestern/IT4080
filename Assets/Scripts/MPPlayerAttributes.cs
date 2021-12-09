using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine.UI;

public class MPPlayerAttributes : NetworkBehaviour
{
    public string playerName;
    public Slider healthBar;

    public bool isDead = false;

    private float maxHealth = 100.0f;
    private NetworkVariableFloat currentHealth = new NetworkVariableFloat(100.0f);
    private GameObject ui;
    public GameObject[] deadUIs;
    private int playerIndex;
    private int respawnTimer = 0;
    
    public GameObject spawnPoint;
    
    public NetworkVariableInt score = new NetworkVariableInt(0);
    public NetworkVariableInt lives = new NetworkVariableInt(3);
        
    // Start is called before the first frame update
    void Start()
    {
        score.OnValueChanged += ScoreChanged;
        lives.OnValueChanged += LivesChanged;
        ui = GameObject.FindGameObjectWithTag("Chat UI");
        playerIndex = ui.GetComponent<MPChatUI>().networkPlayers.IndexOf(playerName);
        /*deadUIs = GameObject.FindGameObjectsWithTag("DeadUI");
        foreach(GameObject go in deadUIs) {
            go.SetActive(false);
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.value = currentHealth.Value;        
    }

    void LateUpdate() {
        if (currentHealth.Value <= 0 && !isDead) {
            Debug.Log("You died");
            isDead = true;
            lives.Value -= 1;
            PlayerActiveClientRpc(false);           
        }
    }

    //update to look for bullet spawner id, if exists, do nothing
    void OnCollisionEnter(Collision collision) {
        string tag = collision.gameObject.tag;

        switch(tag) {            
            case "Bullet":
                Debug.Log("I am hit");           
                UpdateHealthServerRpc(-10.0f);
                Destroy(collision.gameObject);       
                break;

            case "Health":            
                Destroy(collision.gameObject);
                UpdateHealthServerRpc(25.0f);
                break;
        }
    }

    [ServerRpc(RequireOwnership=false)]
    private void UpdateHealthServerRpc(float update) {
        currentHealth.Value += update;
        if (currentHealth.Value > maxHealth) {
            currentHealth.Value = maxHealth;
        }
    }

    private void ScoreChanged(int previousValue, int newValue) {
        //GameObject ui = GameObject.FindGameObjectWithTag("Chat UI");
        //int playerIndex = ui.GetComponent<MPChatUI>().networkPlayers.IndexOf(playerName);
        ui.GetComponent<MPChatUI>().networkScores[playerIndex] = newValue.ToString();
    }

    private void LivesChanged(int previousValue, int newValue) {
        ui.GetComponent<MPChatUI>().networkLives[playerIndex] = newValue.ToString();
    }

    [ServerRpc]
    private void PlayerActiveServerRpc(bool active, int index) {
        //PlayerActiveClientRpc(active, index);
    }

    [ClientRpc]
    private void PlayerActiveClientRpc(bool active) {
        //if (playerIndex == index) {
            gameObject.SetActive(active);

            if (isDead && lives.Value > 0) {
                //deadUIs[playerIndex].SetActive(true);
                InvokeRepeating("RespawnCountDown", 0.0f, 1.0f);
                //Respawn();
            } else if (lives.Value <= 0) {
                GameObject.FindGameObjectWithTag("Level Manager").GetComponent<LevelManager>().CheckGameOver();
            }
        //}
    }

    private void RespawnCountDown() {
        //Text respawn = GameObject.FindGameObjectWithTag("RespawnText").GetComponent<Text>();
        //respawn.text += ".";
        respawnTimer += 1;
        if (respawnTimer >= 4) {
            respawnTimer = 0;
            //respawn.text = "respawning";
            CancelInvoke("RespawnCountDown");
            Respawn();
        }
    }

    private void Respawn() {
        //deadUIs[playerIndex].SetActive(false);
        isDead = false;        
        UpdateHealthServerRpc(100);
        gameObject.transform.position = spawnPoint.transform.position;
        //PlayerActiveServerRpc(true, playerIndex);
        PlayerActiveClientRpc(true);
    }
}
