using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyPlayerPanel : MonoBehaviour
{  
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private Image playerIcon;
    [SerializeField] internal Toggle readyToggle;

    internal void UpdatePlayerName(string name) {
        playerName.text = name;
    }

    internal void UpdatePlayerIcon(string icon) {
        if (icon == null)
            playerIcon.sprite = null;
        playerIcon.sprite = Resources.Load<Sprite>(icon);
    }
}
