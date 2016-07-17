//This ovverides some stuff in the network manager, such as applying the player name when spawning.

using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FLARE_NetworkManager : NetworkManager {

    //public GAME_PlayerManager playerManager;

        /*
    void Update()
    {
        if (NetworkServer.active || NetworkClient.active)
        {
            if (GameObject.FindGameObjectWithTag("PlayerManager"))
            {
                playerManager = GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<GAME_PlayerManager>();
            }
        }
    }
    */

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject namedPlayer = Instantiate(playerPrefab);
        namedPlayer.GetComponent<PLAYER_Identity>().playerAlive = false;               
        
        NetworkServer.AddPlayerForConnection(conn, namedPlayer, playerControllerId);

        var newPlayer = conn.playerControllers[0].gameObject;

        GAME_PlayerManager.AddPlayer(newPlayer);
    }

    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    {
        GAME_PlayerManager.RemovePlayer(player.gameObject);
        
        base.OnServerRemovePlayer(conn, player);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        foreach (var p in conn.playerControllers)
        {
            if (p != null && p.gameObject != null)
            {
                GAME_PlayerManager.RemovePlayer(p.gameObject);
            }
        }
        base.OnServerDisconnect(conn);
    }
}
