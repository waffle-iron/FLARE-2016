using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_PlayerList : MonoBehaviour {

    public GameObject playerListObject;
    public GameObject[] players;
    public Transform listTransform;
    public float updateRate = 2;

	void Start()
    {
        InvokeRepeating("refreshPlayerList", 0, updateRate);
       // StartCoroutine("refreshPlayerList", updateRate);
    }

	void refreshPlayerList () {
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach(Transform t in listTransform)
        {
            Destroy(t.gameObject);
        }
        foreach(GameObject g in players)
        {
            GameObject n = Instantiate(playerListObject);
            n.transform.SetParent(listTransform, false);
            n.GetComponent<Text>().text = g.name;

            if (g.GetComponent<PLAYER_Identity>().playerAlive)
            {
                n.GetComponent<Text>().color = Color.black;
            }
            else
            {
                n.GetComponent<Text>().color = Color.red;
            }
        }
        foreach (Transform t in listTransform)
        {
            if(t.name == GAME_PreGameInfo.playername){
                t.SetAsFirstSibling();
            }
        }
        //yield return null;
    }
}
