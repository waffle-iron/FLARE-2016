using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_TeamList : MonoBehaviour {
    
    /*
     
    public GameObject teamListObject;
    public UI_GameMenuManager menuManager;
    public float updateRate = 2;

	void Start()
    {
        InvokeRepeating("refreshTeamList", 0, updateRate);
       // StartCoroutine("refreshPlayerList", updateRate);
    }

	void refreshTeamList () {

        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }

        if (menuManager.player.teams.Count > 0)
        {
            for (int t = 0; t < menuManager.player.teams.Count - 1; t++)
            {
                GameObject n = Instantiate(teamListObject);
                n.transform.SetParent(transform, false);
                n.GetComponent<Text>().text = menuManager.player.teams[t].displayName + " --- " + menuManager.player.teams[t].playerCount.ToString() + "/" + menuManager.player.teams[t].capacity.ToString();
                n.GetComponent<Text>().color = menuManager.player.teams[t].color;

                n.GetComponent<Button>().onClick.AddListener(delegate { menuManager.joinTeam(menuManager.player.teams[t].index); });
            }
        }
    }
    */
}
