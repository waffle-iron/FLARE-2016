using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

//Simple script to keep this object over level changes
public class KeepOnLoad : MonoBehaviour {
	void Awake () {
        DontDestroyOnLoad(this.gameObject);
	}
    void Update()
    {
        SceneManager.MoveGameObjectToScene(this.gameObject, SceneManager.GetActiveScene());
    }
}
