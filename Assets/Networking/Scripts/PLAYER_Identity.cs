using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class PLAYER_Identity : NetworkBehaviour {

    //Specific to this player
    [SyncVar]
    public string playerName; //Duh

    [SyncVar]
    public bool playerAlive; //Duh

    [SyncVar]
    public int playerTeam = -1; //What integer out of the "teams" variable am I? -1 if no team (default).

    //Other things
    public GameObject playerObject; //Physical object for player, contains movement scripts and the actual player the client sees.

	void Start ()
    { 
        //If local player, set name and death status.
        if (isLocalPlayer)
        {    
            CmdSetPlayerName(GAME_PreGameInfo.playername);
            CmdSetPlayerAlive(false);
        }
    }

    void Update() {  
        if (isLocalPlayer)
        {
            //If player alive
            if (playerAlive)
            {
                //Temporary input!
                transform.Translate(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
            }
        }

        //Sets values from syncvars, such as name and alive status.
        gameObject.name = playerName;
        gameObject.tag = "Player";

        if (playerTeam != -1)
        {
                playerObject.SetActive(playerAlive);
        }              
    }

    //Spawning and Dying functions, to be called from other scripts from buttons etc.
    public void spawn()
    {
        if(isLocalPlayer)
        {
            CmdSetPlayerAlive(true);
        }
    }

    public void suicide()
    {
        if(isLocalPlayer)
        {
            CmdSetPlayerAlive(false);
        }
    }

    //Sets name of player on the syncvar
    [Command]
    public void CmdSetPlayerName(string name)
    {
        playerName = name;
        playerTeam = -1;
    }

    //Sets player alive syncvar, unless there is no team assigned (then dead)
    [Command]
    public void CmdSetPlayerAlive(bool state)
    {
        if (playerTeam != -1)
        {
            playerAlive = state;
        }else
        {
            playerAlive = false;
        }
    }

    public void setTeam(int index)
    {
        if (isLocalPlayer)
        {
            CmdSetPlayerTeam(index);
        }
    }

    [Command]
    public void CmdSetPlayerTeam(int teamIndex)
    {
        //Send message to the Team Manager      
    }
}
