using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_GameMenuManager : MonoBehaviour {

    GameObject playerObject;
    [HideInInspector]
    public PLAYER_Identity player;

    public GameObject teamMenu;
    public GameObject teamCreateMenu;
        public InputField teamDisplayName;
        public Slider teamCapacity;
        public Dropdown teamColor;
    public Text sliderText;

    public GameObject spawnMenu;

    public UI_PlayerList playerList;
    public Text playerCount;

	void Update () {
        if(playerObject == null)
        {
            teamMenu.SetActive(false);
            spawnMenu.SetActive(false);
            findPlayer();
        }else
        {
            if(player.playerTeam >= 0)
            {
                spawnMenu.SetActive(true);
                teamMenu.SetActive(false);
            }else
            {
                sliderText.text = "Team size: " + teamCapacity.value;
                spawnMenu.SetActive(false);
                teamMenu.SetActive(true);
            }

            playerCount.text = player.gameCapacity.ToString();
        }
	}

    public void joinTeam(int teamIndex)
    {
        player.setTeam(teamIndex);
    }

    public void leaveTeam()
    {
        player.suicide();
        player.setTeam(-1);
    }

    public void createTeamMenu()
    {
        teamCreateMenu.SetActive(!teamCreateMenu.active);
    }

    public void createAndJoinTeam()
    {
        Color tCol = Color.clear;

        switch (teamColor.value.ToString())
        {
            case "RED":
                tCol = Color.red;
                break;
            case "BLUE":
                tCol = Color.blue;
                break;
            case "GREEN":
                tCol = Color.green;
                break;
            case "YELLOW":
                tCol = Color.yellow;
                break;
            case "PINK":
                tCol = Color.magenta;
                break;
        }
        player.createTeam(teamDisplayName.text, int.Parse(teamCapacity.value.ToString()), tCol);
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

    void findPlayer()
    {
        foreach(GameObject g in playerList.players)
        {
            if(g.name == GAME_ClientInfo.playername)
            {
                playerObject = g;
                player = g.GetComponent<PLAYER_Identity>();
            }
        }
    }
}
