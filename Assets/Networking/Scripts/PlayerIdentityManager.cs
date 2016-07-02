using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerIdentityManager : NetworkBehaviour {

    [SyncVar]
    public string playerName;
    [SyncVar]
    public bool playerAlive;

	// Update is called once per frame
	void Update () {
	    if(isLocalPlayer)
        {
            CmdSetPlayerName(PreLoadInfo.playername);
            if (Input.GetKeyDown(KeyCode.K))
            {
                CmdSetPlayerAlive(!playerAlive);
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
    }
}
