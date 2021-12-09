using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI.Serialization;

public struct PlayerColor : MLAPI.Serialization.INetworkSerializable
{
    public string material;
    public string icon;

    public PlayerColor(string material, string icon) {
        this.material = material;
        this.icon = icon;
    }

    public void NetworkSerialize(NetworkSerializer serializer) {
        serializer.Serialize(ref material);
        serializer.Serialize(ref icon);
    }
}
