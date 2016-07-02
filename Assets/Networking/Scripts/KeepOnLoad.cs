using UnityEngine;
using System.Collections;

//Simple script to keep this object over level changes
public class KeepOnLoad : MonoBehaviour {
	void Awake () {
        DontDestroyOnLoad(this.gameObject);
	}
}
