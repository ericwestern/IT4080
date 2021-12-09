using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoController : NetworkBehaviour
{
    public Text pName;
    public Text score;
    public Text lives;
    public string playerName;

    // Start is called before the first frame update
    void Start()
    {
        pName.text = playerName;
        score.text = "0";
        lives.text = "3";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ServerRpc]
    public void UpdateScoreServerRpc(int val) {
        score.text = val.ToString();
    }
}
