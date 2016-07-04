using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class GAME_ClientInfo : MonoBehaviour {

    public InputField nameEntryField;
    public static string playername;
    public string mainMenuScene;

    public void setName () {
        if (!IsNullOrWhiteSpace(nameEntryField.text))
        {
            playername = RemoveWhiteSpace(nameEntryField.text);
            SceneManager.LoadScene(mainMenuScene);
        }
	}

    public string RemoveWhiteSpace(string value)
    {
        return Regex.Replace(value, "( )+", "");
    }

    public static bool IsNullOrWhiteSpace(string value)
    {
        if (value == null) return true;

        for (int i = 0; i < value.Length; i++)
        {
            if (!char.IsWhiteSpace(value[i])) return false;
        }

        return true;
    }
}
 