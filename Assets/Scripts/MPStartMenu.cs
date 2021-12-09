using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MLAPI;
using MLAPI.SceneManagement;

public class MPStartMenu : NetworkBehaviour
{
    public GameObject startMenu;

    [SerializeField] private TMP_InputField playerName;

    public void HostButtonClicked() {
        PlayerPrefs.SetString("PlayerName", playerName.text);
        NetworkManager.Singleton.StartHost();
        NetworkSceneManager.SwitchScene("Lobby");
    }

    public void ClientButtonClicked() {
        PlayerPrefs.SetString("PlayerName", playerName.text);
        NetworkManager.Singleton.StartClient();
    }

    public void ExitButtonClicked() {
        Application.Quit();
    }
}
