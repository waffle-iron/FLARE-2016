using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_GameMenuManager : MonoBehaviour {

    GameObject playerObject;
    //[HideInInspector]
    public PLAYER_Identity player;

    public Text sliderText;

    public GameObject spawnMenu;

    public UI_PlayerList playerList;
    public Text playerCount;

	void Update () {
        if(playerObject == null)
        {
            spawnMenu.SetActive(false);
            findPlayer();
        }else
        {
            //If the player has a team show the spawn menu
            if(player.playerTeam >= 0)
            {
                spawnMenu.SetActive(true);
            }else
            {
                spawnMenu.SetActive(false);
            }
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

    void findPlayer()
    {
        foreach(GameObject g in playerList.players)
        {
            if(g.name == GAME_PreGameInfo.playername)
            {
                playerObject = g;
                player = g.GetComponent<PLAYER_Identity>();
            }
        }
    }
}
