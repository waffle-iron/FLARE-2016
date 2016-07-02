using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PreLoadInfo : MonoBehaviour {

    public InputField nameEntryField;
    public static string playername;
    public string mainMenuScene;

    public void setName () {
        SceneManager.LoadScene(mainMenuScene);
        playername = nameEntryField.text;
	}
}
 