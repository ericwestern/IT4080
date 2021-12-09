using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Connection;
using MLAPI.Messaging;
using MLAPI.NetworkVariable.Collections;
using MLAPI.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class MPLobby : NetworkBehaviour
{
    public Button startGameButton;

    [SerializeField] private LobbyPlayerPanel[] lobbyPlayers;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject chatUIPrefab;
    [SerializeField] private GameObject playerUIPrefab;

    private NetworkList<MPPlayerInfo> networkPlayers = new NetworkList<MPPlayerInfo>();
    private List<string> colors = new List<string>() {"red", "blue"};
    private int playerCount = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        UpdateConnListServerRpc(NetworkManager.LocalClientId);
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
    }

    public override void NetworkStart()
    {
        if (IsClient) {
            networkPlayers.OnListChanged += PlayersInfoChanged;
        }
        if (IsServer) {
            NetworkManager.Singleton.OnClientConnectedCallback += ClientConnectedHandle;
            NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnectedHandle;

            foreach(NetworkClient client in NetworkManager.Singleton.ConnectedClientsList) {
                ClientConnectedHandle(client.ClientId);
            }
        }
    }

    private void OnDestroy() {
        networkPlayers.OnListChanged -= PlayersInfoChanged;
        if (NetworkManager.Singleton) {
            NetworkManager.Singleton.OnClientConnectedCallback -= ClientConnectedHandle;
            NetworkManager.Singleton.OnClientDisconnectCallback -= ClientDisconnectedHandle;
        }
    }    

    private void PlayersInfoChanged(NetworkListEvent<MPPlayerInfo> changeEvent)
    {
        int index = 0;
        foreach(MPPlayerInfo player in networkPlayers) {
            lobbyPlayers[index].UpdatePlayerName(player.networkPlayerName);
            lobbyPlayers[index].UpdatePlayerIcon(PlayerColors.GetPlayerColor(player.networkPlayerColor).icon);
            lobbyPlayers[index].readyToggle.SetIsOnWithoutNotify(player.networkPlayerReady);
            index++;
        }

        for(; index < 3; index++) {
            lobbyPlayers[index].UpdatePlayerName("Player");
            lobbyPlayers[index].UpdatePlayerIcon(null);
            lobbyPlayers[index].readyToggle.SetIsOnWithoutNotify(false);
        }

        if(IsHost)
            startGameButton.interactable = CheckEveryoneReady();
    }

    private bool CheckEveryoneReady() {
        Debug.Log("Checking for ready");
        foreach (MPPlayerInfo p in networkPlayers) {
            if (!p.networkPlayerReady)
                return false;
        }
        
        return true;
    }

    public void StartGame() {
        if (IsServer) {
            NetworkSceneManager.OnSceneSwitched += SceneSwitched;
            
            NetworkSceneManager.SwitchScene("MainGame");
        } else {
            Debug.Log("You are not the host");
        }
    }

    private void SceneSwitched() {
        GameObject manager = GameObject.FindGameObjectWithTag("Level Manager");
        GameObject ui = GameObject.FindGameObjectWithTag("Chat UI");
        LevelManager levelManager = manager.GetComponent<LevelManager>();
        
        int i = 0;

        foreach(MPPlayerInfo player in networkPlayers) {
            GameObject playerSpawn = Instantiate(playerPrefab, levelManager.playerSpawns[i].transform.position, Quaternion.identity);
            playerSpawn.GetComponent<NetworkObject>().SpawnWithOwnership(player.networkClientId);
            playerSpawn.GetComponent<MPPlayerAttributes>().playerName = player.networkPlayerName;
            playerSpawn.GetComponent<MPPlayerAttributes>().spawnPoint = levelManager.playerSpawns[i];


            ui.GetComponent<MPChatUI>().networkPlayers.Add(player.networkPlayerName);
            ui.GetComponent<MPChatUI>().networkScores.Add(playerSpawn.GetComponent<MPPlayerAttributes>().score.Value.ToString());
            ui.GetComponent<MPChatUI>().networkLives.Add(playerSpawn.GetComponent<MPPlayerAttributes>().lives.Value.ToString());
            
            i++;
        }
    }

    private void HandleClientConnected(ulong clientId) {
        UpdateConnListServerRpc(clientId);
    }

    [ServerRpc]
    private void UpdateConnListServerRpc(ulong clientId) {
        networkPlayers.Add(new MPPlayerInfo(clientId, PlayerPrefs.GetString("PlayerName"), false, colors[playerCount]));
        playerCount++;
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateReadyStatusServerRpc(ServerRpcParams serverRpcParams = default) {
        for (int i = 0; i < networkPlayers.Count; i++) {
            if (networkPlayers[i].networkClientId == serverRpcParams.Receive.SenderClientId) {
                networkPlayers[i] = new MPPlayerInfo(networkPlayers[i].networkClientId,
                                                    networkPlayers[i].networkPlayerName,
                                                    !networkPlayers[i].networkPlayerReady,
                                                    networkPlayers[i].networkPlayerColor);
                return;
            }
        }
    }

    public void ReadyStatusChanged() {
        UpdateReadyStatusServerRpc();
    }
    
    private void ClientDisconnectedHandle(ulong clientId)
    {
        MPPlayerInfo p = networkPlayers.Where(p => p.networkClientId == clientId).First();
        networkPlayers.Remove(p);
    }

    private void ClientConnectedHandle(ulong obj)
    {
        //throw new NotImplementedException();
    }
}
