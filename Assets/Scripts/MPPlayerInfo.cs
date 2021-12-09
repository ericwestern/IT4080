using System.Collections;
using System.Collections.Generic;
using MLAPI.Serialization;
using UnityEngine;

public struct MPPlayerInfo : MLAPI.Serialization.INetworkSerializable
{
    public ulong networkClientId;
    public string networkPlayerName;
    public bool networkPlayerReady;
    public string networkPlayerColor;

    public MPPlayerInfo(ulong clientId, string name, bool ready, string color) {
        networkClientId = clientId;
        networkPlayerName = name;
        networkPlayerReady = ready;
        networkPlayerColor = color;
    }

    public void NetworkSerialize(NetworkSerializer serializer) {
        serializer.Serialize(ref networkClientId);
        serializer.Serialize(ref networkPlayerName);
        serializer.Serialize(ref networkPlayerReady);
        serializer.Serialize(ref networkPlayerColor);
    }
}