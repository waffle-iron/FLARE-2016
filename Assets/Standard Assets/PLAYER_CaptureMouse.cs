using UnityEngine;
using System.Collections;

public class PLAYER_CaptureMouse : MonoBehaviour
{

	public static bool caught = false;
	public bool forceOff = false;

	void Start ()
	{
		caught = true;
	}

	void Update ()
	{
		if (caught && !forceOff) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		} else {
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}
}
