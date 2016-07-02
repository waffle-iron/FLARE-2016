using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NetworkPlayerList : MonoBehaviour {

    public GameObject playerListObject;
    public GameObject[] players;

	// Update is called once per frame
	void Update () {
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach(Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        foreach(GameObject g in players)
        {
            GameObject n = Instantiate(playerListObject);
            n.transform.SetParent(transform, false);
            n.GetComponent<Text>().text = g.name;
            if (g.GetComponent<PlayerIdentityManager>().playerAlive)
            {
                n.GetComponent<Text>().color = Color.black;
            }
            else
            {
                n.GetComponent<Text>().color = Color.red;
            }
        }
	}
}
