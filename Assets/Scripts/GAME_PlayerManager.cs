using UnityEngine;
using UnityEngine.Networking;

public class GAME_PlayerManager : NetworkBehaviour
{
    //Information for every CLIENT (to access their corresponding objects)
    public struct ClientInfo
    {
        public string name;
        public float added;
        public NetworkInstanceId netId;
        public override string ToString()
        {
            return name + " (" + netId + ")";
        }
    };
    public class SyncListClientInfo : SyncListStruct<ClientInfo> { }

    //Info for every PLAYER (the objects of the client!)
    public struct PlayerInfo
    {
        public string name;
        public int team;
        public int kills, deaths;
        public bool alive;
        //public int latency;
    };
    public class SyncListPlayerInfo : SyncListStruct<PlayerInfo> { }

    //Info for the teams on the server
    public struct TeamInfo
    {
        public string name;
        public int index;
        public int capacity;
        public int memberCount;
        public bool isFull;
    }
    public class SyncListTeamInfo : SyncListStruct<TeamInfo> { }

    [SyncVar]
    public static SyncListClientInfo clientList = new SyncListClientInfo();
    [SyncVar]
    public static SyncListPlayerInfo playerList = new SyncListPlayerInfo();
    [SyncVar]
    public static SyncListTeamInfo teamList = new SyncListTeamInfo();

    public static string[] teamNames = { "RED", "BLUE", "GREEN", "YELLOW" };

    void ClientChanged(SyncListStruct<ClientInfo>.Operation op, int itemIndex)
    {
        Debug.Log("Players changed:" + op);
        RefreshPlayerList();
    }

    void Start()
    {
        clientList.Callback = ClientChanged;
        RefreshPlayerList();
    }

    public static void UpdateLists()
    {
        RefreshPlayerList();
    }

    public static void RefreshPlayerList()
    {
        playerList.Clear(); //Clear the list

        //Populate the list
        foreach (ClientInfo c in clientList)
        {
            if (c.name != null)
            {
                PLAYER_Identity playerObj = ClientScene.FindLocalObject(c.netId).GetComponent<PLAYER_Identity>();
                PlayerInfo player = new PlayerInfo();
                player.alive = playerObj.playerAlive;
                player.deaths = playerObj.deaths;
                player.kills = playerObj.kills;
                player.name = playerObj.playerName;
                player.team = playerObj.playerTeam;
                playerList.Add(player);
            }
        }

        //Then populate the team list
        RefreshTeamList();
    }

    public static void RefreshTeamList()
    {
        teamList.Clear(); //Clear list
        
        //Populate list
        for(int i = 0; i < GAME_PreGameInfo.amountOfTeams; i++)
        {
            TeamInfo team = new TeamInfo();
            team.name = teamNames[i];
            team.index = teamList.Count;
            team.capacity = Mathf.FloorToInt(64 / GAME_PreGameInfo.amountOfTeams);
            teamList.Add(team);
        }

        foreach (PlayerInfo player in playerList)
        {
            if (player.team != -1) {
                TeamInfo team = teamList[player.team];
                team.memberCount++;

                if (team.memberCount == team.capacity)
                {
                    team.isFull = true;
                }else
                {
                    team.isFull = false;
                }

                teamList[player.team] = team;
            }
        }
    }


    public static void AddPlayer(GameObject player)
    {
        if (player == null)
        {
            Debug.LogError("AddPlayer null");
            return;
        }

        NetworkIdentity uv = player.GetComponent<NetworkIdentity>();
        if (uv == null)
        {
            Debug.LogError("AddPlayer no network identity");
            return;
        }

        ClientInfo b = new ClientInfo();
        b.name = player.name;
        b.netId = uv.netId;
        b.added = Time.time;

        clientList.Add(b);
    }

    public static void RemovePlayer(GameObject player)
    {
        if (player == null)
        {
            Debug.LogError("RemovePlayer null");
            return;
        }

        var uv = player.GetComponent<NetworkIdentity>();
        if (uv == null)
        {
            Debug.LogError("RemovePlayer no network identity");
            return;
        }

        var netId = uv.netId;
        foreach (var p in clientList)
        {
            if (netId == p.netId)
            {
                clientList.Remove(p);
                break;
            }
        }
    }

    public static bool CheckPlayerExists(string nameToCheck)
    {
        foreach(ClientInfo p in clientList)
        {
            if(p.name == nameToCheck)
            {
               return false;
            }
        }
        return true;
    }
}