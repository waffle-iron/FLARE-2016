using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class PLAYER_Identity : NetworkBehaviour {

    //Specific to this player
    [HideInInspector]
    [SyncVar]
    public string playerName; //Duh
    [HideInInspector]
    [SyncVar]
    public bool playerAlive; //Duh

    [SyncVar]
    public int playerTeam = -1; //What integer out of the "teams" variable am I? -1 if no team (default).
    public string playerTeamName; //display name of team

    //Synchronised list of teams
    [SyncVar]
    public SyncListTeam teams = new SyncListTeam();

    //Team struct for creating managing teams etc.
    public struct Team
    {
        public string displayName;
        public int capacity;
        public int playerCount;
        public Color color;
        public int index;
    }

    public class SyncListTeam : SyncListStruct<Team> { }

    //Other things
    public GameObject playerObject; //Physical object for player, contains movement scripts and the actual player the client sees.
    FLARE_NetworkManager NET; //Network manager (only used from server)
    public string gameCapacity; //Connections out of max connections

	void Start ()
    {
        //If server, then send updates about game full-ness
        if (isServer)
        {
            NET = GameObject.FindWithTag("GameController").GetComponent<FLARE_NetworkManager>();
            InvokeRepeating("RefreshPlayerList", 0, 2);
        }

        //If local player, set name and death status.
        if (isLocalPlayer)
        {    
            CmdSetPlayerName(GAME_ClientInfo.playername);
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

        if (playerTeam >= 0)
        {
            //if (!teams.GetItem(playerTeam).Equals(null))
            //{
               // playerTeamName = teams[playerTeam].displayName;
               // playerObject.GetComponent<Renderer>().material.SetColor("_Color", teams[playerTeam].color);
                playerObject.SetActive(playerAlive);
            //}
        }              
    }

    //Called on server to send the player count to everyone
    [Server]
    public void RefreshPlayerList()
    {
        RpcRefreshPlayerList(NET.numPlayers.ToString() + " / " + NET.maxConnections.ToString());
    }
    //Called on client to recieve the playercount and update "gameCapacity" variable.
    [ClientRpc]
    public void RpcRefreshPlayerList(string amount)
    {
        gameCapacity = amount;
    }

    [ClientRpc]
    void RpcUpdateTeams(SyncListTeam t)
    {
        teams = t;
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


    public void createTeam(string n, int m, Color c)
    {
        if(isLocalPlayer)
        {
            CmdCreateTeam(n, m, c);
        }
    }

    [Command]
    public void CmdCreateTeam(string display, int max, Color col)
    {
        //if (max == 0) { max = 1; }

        Team teamToAdd = new Team
        {
            displayName = display,
            index = teams.Count,
            capacity = max,
            color = col
        };
        teams.Add(teamToAdd);
        CmdSetPlayerTeam(teamToAdd.index);
        RpcUpdateTeamList(teams);
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
        if (teamIndex >= 0)
        {
            Team updatedTeam = teams[teamIndex];

            if (updatedTeam.capacity <= updatedTeam.playerCount) //Team full, do nothing
            {
                playerTeam = -1;
            }
            else
            {
                updatedTeam.playerCount++;
                teams[teamIndex] = updatedTeam;
                playerTeam = teamIndex;
            }
        }else
        {
            Team updatedTeam = teams[playerTeam];
            updatedTeam.playerCount--;

            if(updatedTeam.playerCount == 0)
            {
                teams[playerTeam] = updatedTeam;
                teams.RemoveAt(playerTeam);
            }else
            {
                teams[playerTeam] = updatedTeam;
            }

            playerTeam = -1;
        }

        RpcUpdateTeamList(teams);        
    }

    [ClientRpc]
    public void RpcUpdateTeamList(SyncListTeam t)
    {
        teams = t;
    }

}
