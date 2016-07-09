//This ovverides some stuff in the network manager, such as applying the player name when spawning.

using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FLARE_NetworkManager : NetworkManager {

    public GAME_PlayerManager playerManager;

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject namedPlayer = Instantiate(playerPrefab);
        namedPlayer.GetComponent<PLAYER_Identity>().playerAlive = false;               
        
        NetworkServer.AddPlayerForConnection(conn, namedPlayer, playerControllerId);

        var newPlayer = conn.playerControllers[0].gameObject;

        playerManager.AddPlayer(newPlayer);
    }

    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    {
        playerManager.RemovePlayer(player.gameObject);
        
        base.OnServerRemovePlayer(conn, player);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        foreach (var p in conn.playerControllers)
        {
            if (p != null && p.gameObject != null)
            {
                playerManager.RemovePlayer(p.gameObject);
            }
        }
        base.OnServerDisconnect(conn);
    }
}
