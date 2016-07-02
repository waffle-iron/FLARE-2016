//Analyses the environment and adjusts UI accordingly.

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIStateManager : MonoBehaviour {
    public GameObject[] uiParents;

	void Update () {
        setUIState(SceneManager.GetActiveScene().name);                        
	}

    void setUIState(string activeScene)
    {
        foreach(GameObject s in uiParents)
        {
            if(s.name == activeScene)
            {
                s.SetActive(true);
            }else
            {
                s.SetActive(false);
            }
        }
    }
}
