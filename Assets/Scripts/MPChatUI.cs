using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MPChatUI : NetworkBehaviour
{
    public Text chatText;
    public InputField inputText;
    public GameObject[] chatPlayers;

    public Text[] playerNames;
    public Text[] playerScores;
    public Text[] playerLives;

    public NetworkList<string> networkPlayers = new NetworkList<string>();
    public NetworkList<string> networkScores = new NetworkList<string>();
    public NetworkList<string> networkLives = new NetworkList<string>();

    private NetworkList<string> messageLog = new NetworkList<string>();

    // Start is called before the first frame update
    void Start()
    {
        chatPlayers = GameObject.FindGameObjectsWithTag("Player");
    }

    public override void NetworkStart() {
        messageLog.OnListChanged += MessageLogChanged;
        networkPlayers.OnListChanged += PlayersChanged;
        networkScores.OnListChanged += ScoresChanged;
        networkLives.OnListChanged += LivesChanged;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSend() {
        SendMessageServerRpc(inputText.text);
        inputText.text = "";
    }

    private void MessageLogChanged(NetworkListEvent<string> changeEvent) {
        chatText.text = "";
        foreach (string message in messageLog)
        {
            chatText.text += (message + "\n");
        }
    }

    private void PlayersChanged(NetworkListEvent<string> changeEvent) {
        int i = 0;
        foreach (string player in networkPlayers)
        {
            playerNames[i].text = player;
            i++;
        }
    }

    private void ScoresChanged(NetworkListEvent<string> changeEvent) {
        int i = 0;
        foreach (string score in networkScores)
        {
            playerScores[i].text = score;
            i++;
        }
    }

    private void LivesChanged(NetworkListEvent<string> changeEvent) {
        int i = 0;
        foreach (string lives in networkLives)
        {
            playerLives[i].text = lives;
            i++;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendMessageServerRpc(string message, ServerRpcParams svrParams = default) {
        Debug.Log("Sending message");

        string playerName = "Player";
        
        foreach(GameObject p in chatPlayers) {
            if (p.GetComponent<NetworkObject>().OwnerClientId == svrParams.Receive.SenderClientId)
                playerName = p.GetComponent<MPPlayerAttributes>().playerName;
        }

        message = playerName + ": " + message;

        if (messageLog.Count < 8) {
            messageLog.Add(message);
        } else {
            messageLog.RemoveAt(0);
            messageLog.Add(message);
        }
    }
}
