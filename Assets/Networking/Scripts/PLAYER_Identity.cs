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
    public int kills, deaths; //Amount of kills and deaths.
    [SyncVar]
    public int playerTeam = -1; //What integer out of the "teams" variable am I? -1 if no team (default).
    public teamStruct playerTeamDebug;
    [SyncVar]
    public int playerTeamIndex; //What position in the team are we
    [SyncVar]
    public bool playerIsHost = false;
    [SyncVar]
    public string hostingPlayer;

    //Syncvar Teams
    public teamStructSync teams = new teamStructSync();
    public playerStructSync teamMembers = new playerStructSync();

    //Team construction stuffs
    public class teamStructSync : SyncListStruct<teamStruct> { }

    [System.Serializable]
    public struct teamStruct
    {
        public string name;
        public int index;
        public Color color;
        public int capacity;
    }

    public class playerStructSync : SyncListStruct<playerStruct> { }

    [System.Serializable]
    public struct playerStruct
    {
        public string name;
        public int team;
        public int index;
        public int kills;
        public int deaths;
    }

    //Other things
    public float teamRefreshRate = 2.0f;
    public GameObject playerObject; //Physical object for player, contains movement scripts and the actual player the client sees.

	void Start ()
    { 
        //If local player, set name and death status.
        if (isLocalPlayer)
        {    
            //Set the name syncvar for our player object over the network
            CmdSetPlayerName(GAME_PreGameInfo.playername);
            //Set the player alive state for our player object over the network
            CmdSetPlayerAlive(false);
            //Set if we are the host or not, for others to see
            CmdSetPlayerHost(isServer);
            //Get the hosting player! (for us to use later, to get teams etc)
            CmdGetHostingPlayer();

            //If we are the server we do special things like setup teams
            if (isServer)
            {
                //Debug.Log("ISSERVER RUNS WITHIN ISLOCALPLAYER WOO!");
                CmdCreateTeams(GAME_PreGameInfo.amountOfTeams);
            }else
            {
                InvokeRepeating("CmdGetTeamsFromHost",0,teamRefreshRate);
            }
        }
    }

    void Update() {  

        if (isLocalPlayer)
        {
            if (playerTeam == -1)
            {
                CmdAttemptTeamAssign(0);
            }
            else
            {
                playerTeamDebug.name = teams[playerTeam].name;
                playerTeamDebug.color = teams[playerTeam].color;
                playerTeamDebug.capacity = teams[playerTeam].capacity;
            }

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

        //If we have a valid team, then
        if (playerTeam != -1)
        {
                //Set active depending on player state
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

    //Sets the player isHost variable across the network
    [Command]
    public void CmdSetPlayerHost(bool state)
    {
        playerIsHost = state;
    }

    //Gets the player that is hosting
    [Command]
    public void CmdGetHostingPlayer()
    {
        if (!playerIsHost)
        {
            foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (g.GetComponent<PLAYER_Identity>().playerIsHost)
                {
                    hostingPlayer = g.name;
                    break;
                }
            }
        }else
        {
            hostingPlayer = playerName;
        }
    }

    //Creates teams on the hosting player, to be grabbed by everyone else who needs them
    [Command]
    public void CmdCreateTeams(int amount)
    {
        for(int i = 0; i < amount; i ++)
        {
            teamStruct newT = new teamStruct();
            newT.name = "ITS A TEAM!";
            newT.color = new Color(Random.Range(0, 255), Random.Range(0, 255), Random.Range(0, 255));
            newT.capacity = 64 / GAME_PreGameInfo.amountOfTeams;
            newT.index = i;

            teams.Add(newT);
        }
    }

    //Updates the teams syncvar from the hosting player, on the server side (so it 'trickles down')
    [Command]
    public void CmdGetTeamsFromHost()
    {
        if(!playerIsHost)
        {  
            //Ugh the performance. ugh....
            teams = GameObject.Find(hostingPlayer).GetComponent<PLAYER_Identity>().teams;
            teamMembers = GameObject.Find(hostingPlayer).GetComponent<PLAYER_Identity>().teamMembers;
        }
    }

    //Attempts to assign a player to a team! :D
    [Command]
    public void CmdAttemptTeamAssign(int team)
    {
        playerStruct newP = new playerStruct();
        newP.name = playerName;
        newP.kills = kills;
        newP.deaths = deaths;
        newP.team = team;
        newP.index = teamMembers.Count;

        int teamCount = 0;
        foreach(playerStruct p in teamMembers)
        {
            if(p.team == team)
            {
                teamCount++;
            }
            if(p.name == playerName)
            {
                teamCount += 1000; //YOU SHALL NOT JOIN
            }
        }

        if (teamCount < teams[team].capacity)
        {
            teamMembers.Add(newP);
            playerTeam = team;
            playerTeamIndex = newP.index;        
        }
    }

    //Update a player's information 
    [Command]
    public void CmdUpdatePlayerStats()
    {
        playerStruct newP = new playerStruct();
        newP.name = playerName;
        newP.kills = kills;
        newP.deaths = deaths;
        newP.team = playerTeam;
        newP.index = playerTeamIndex;

        teamMembers[playerTeamIndex] = newP;
    }
}
