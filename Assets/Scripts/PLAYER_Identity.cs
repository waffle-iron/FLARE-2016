using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class PLAYER_Identity : NetworkBehaviour {

    //Specific to this player
    [SyncVar]
    public string playerName;
    [SyncVar]
    public int playerTeam = -1; 
    [SyncVar]
    public bool playerAlive; 
    [SyncVar]
    public float playerHealth = 100.0f;
    [SyncVar]
    public int kills, deaths; //Amount of kills and deaths.

    public MonoBehaviour playerObject; //Physical object for player, contains movement scripts and the actual player the client sees.
    public GameObject playerObjects;
    public PLAYER_CaptureMouse playerMouse;
    public GameObject clientcanvas;
    public MeshRenderer visiblePlayerObject;

    public GAME_PlayerManager GPM;

    public override void OnStartLocalPlayer ()
    {
        //If local player, set name and death status.
        if (isLocalPlayer)
        {    
            //Set the name syncvar for our player object over the network
            CmdSetPlayerName(GAME_PreGameInfo.playername);
            //Set the player alive state for our player object over the network
            CmdSetPlayerAlive(false);
        }
    }

    void Update() {

        if (GameObject.FindGameObjectWithTag("PlayerManager"))
        {
            GPM = GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<GAME_PlayerManager>();
        }

        if (isLocalPlayer)
        {
            //If player alive
            if (playerAlive)
            {
                if(playerTeam == -1)
                {
                    CmdSetPlayerAlive(false);
                }
            }
            playerObject.enabled = playerAlive;
            playerObjects.SetActive(playerAlive);
            clientcanvas.SetActive(playerAlive);
            visiblePlayerObject.enabled = false;
            playerMouse.forceOff = !playerAlive;
        }else
        {
            playerObjects.SetActive(false);
            playerObject.enabled = false;
            clientcanvas.SetActive(false);
            visiblePlayerObject.enabled = playerAlive;
        }

        //Sets values from syncvars, such as name and alive status.
        gameObject.name = playerName;
        gameObject.tag = "Player";
            
    }

    //Spawning and Dying functions, to be called from other scripts from buttons etc.
    public void spawn()
    {
        if(isLocalPlayer && !playerAlive)
        {
            CmdSetPlayerAlive(true);
        }
    }

    public void suicide()
    {
        if(isLocalPlayer && playerAlive)
        {
            CmdSetPlayerAlive(false);
            CmdAddKillDeath(0, 1);
        }
    }

    public void joinTeam(int teamIndex)
    {
        if (isLocalPlayer && playerTeam == -1)
        {
            CmdAttemptTeamAssign(teamIndex);
        }
    }

    public void leaveTeam()
    {
        if(isLocalPlayer && playerTeam != -1)
        {
            CmdAttemptTeamAssign(-1);
        }
    }

    //Sets name of player on the syncvar
    [Command]
    public void CmdSetPlayerName(string name)
    {
        playerName = name;
        playerTeam = -1;
        if(GPM != null) { GPM.UpdateLists(); }
    }

    //Sets player alive syncvar, unless there is no team assigned (then dead)
    [Command]
    public void CmdSetPlayerAlive(bool state)
    {
        playerAlive = state;
        if (GPM != null) { GPM.UpdateLists(); }
    }

    [Command]
    public void CmdAddKillDeath(int k, int d)
    {
        kills += k;
        deaths += d;
        if (GPM != null) { GPM.UpdateLists(); }
    }

    [Command]
    public void CmdAttemptTeamAssign(int teamIndex)
    {
        if (teamIndex != -1)
        {
            if (!GPM.teamList[teamIndex].isFull)
            {
                playerTeam = teamIndex;
                if (GPM != null) { GPM.UpdateLists(); }
            }
            else
            {
                playerTeam = -1;
            }
        }else
        {
            playerTeam = -1;
            if (GPM != null) { GPM.UpdateLists(); }
        }
    }
}
