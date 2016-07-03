using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerIdentityManager : NetworkBehaviour {

    [SyncVar]
    public string playerName;
    [SyncVar]
    public bool playerAlive;

    public GameObject visiblePlayer;

	void Start ()
    {
        if(isLocalPlayer)
        {
            CmdSetPlayerAlive(false);
        }
        visiblePlayer.SetActive(playerAlive);
    }

	void Update () {
        if (isLocalPlayer)
        {
            CmdSetPlayerName(PreLoadInfo.playername);
            if (Input.GetKeyDown(KeyCode.K))
            {
                CmdSetPlayerAlive(!playerAlive);
            }
            if (playerAlive)
            {
                transform.Translate(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
            }
        }
        gameObject.name = playerName;
	}

    [Command]
    public void CmdSetPlayerName(string name)
    {
        playerName = name;
    }

    [Command]
    public void CmdSetPlayerAlive(bool state)
    {
        playerAlive = state;
        visiblePlayer.SetActive(state);
        RpcSetPlayerAlive(state);
    }

    [ClientRpc]
    public void RpcSetPlayerAlive(bool state)
    {
        visiblePlayer.SetActive(state);
    }
}
