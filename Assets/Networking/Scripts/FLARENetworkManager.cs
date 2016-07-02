﻿//This ovverides some stuff in the network manager, such as applying the player name when spawning.

using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FLARENetworkManager : NetworkManager {

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        //Find team of player etc here!
        GameObject namedPlayer = Instantiate(playerPrefab);
        namedPlayer.GetComponent<PlayerIdentityManager>().playerAlive = false;
        //namedPlayer.gameObject.name = extraMessageReader.ReadString();
        NetworkServer.AddPlayerForConnection(conn, namedPlayer, playerControllerId);
        //base.OnServerAddPlayer(conn, playerControllerId);
    }
}
