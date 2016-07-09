using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class UI_GameMenuManager : MonoBehaviour {

    [HideInInspector]
    GameObject playerObject;
    public PLAYER_Identity player;
    public GAME_PlayerManager GPM;

    public GameObject spawnMenu;
    public GameObject teamMenu;

    public Transform playerList;
    public Transform teamList;
    public GameObject playerListObj;
    public GameObject teamListObj;

    public Text playerCount;

    void Start()
    {
        InvokeRepeating("updateTeamList", 0.5f, 1.0f);
    }


    void Update () {

        if (GPM == null)
        {
            GPM = GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<GAME_PlayerManager>();
        }

        if (playerObject == null)
        {
            teamMenu.SetActive(false);
            spawnMenu.SetActive(false);
            findPlayer();
        }
        else
        {
            teamMenu.SetActive(true);
            spawnMenu.SetActive(true);
        }
	}

    void updateTeamList()
    {
        foreach(Transform t in playerList)
        {
            Destroy(t.gameObject);
        }
        foreach(Transform t2 in teamList)
        {
            Destroy(t2.gameObject);
        }

        foreach(GAME_PlayerManager.PlayerInfo p in GPM.playerList)
        {
            GameObject newP = Instantiate(playerListObj);
            newP.transform.SetParent(playerList, false);
            newP.GetComponent<Text>().text = p.name + " " + p.kills.ToString() + "/" + p.deaths.ToString() + " " + p.alive.ToString();
        }

        foreach(GAME_PlayerManager.TeamInfo t in GPM.teamList)
        {
            GameObject newT = Instantiate(teamListObj);
            newT.transform.SetParent(teamList, false);
            newT.GetComponent<Text>().text = t.name + " " + t.memberCount.ToString() + "/" + t.capacity.ToString();
            newT.GetComponentInChildren<Button>().onClick.AddListener(delegate { joinTeam(t.index); });
        }

    }

    public void suicide()
    {
        if(playerObject != null)
        {
            player.suicide();
        }
    }

    public void spawn()
    {
        if(playerObject != null)
        {
            player.spawn();
        }
    }

    public void joinTeam(int index)
    {
        Debug.Log("Attempting join team " + index);
        if (playerObject != null)
        {
            player.joinTeam(index);
        }
    }

    public void leaveTeam()
    {
        if(playerObject != null)
        {
            player.leaveTeam();
        }
    }

    void findPlayer()
    {
        foreach(GameObject g in GameObject.FindGameObjectsWithTag("Player"))
        {
            if(g.name == GAME_PreGameInfo.playername)
            {
                playerObject = g;
                player = g.GetComponent<PLAYER_Identity>();
            }
        }
    }
}
