using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_RangeUtility : MonoBehaviour {

    [SerializeField]
    private Text rangeDisp;
    [SerializeField]
    private HorizontalLayoutGroup rangeIcon;
    [SerializeField]
    private float rangeIconInterval = 10.0f;
    private float spacerWidth;

    [SerializeField]
    private Camera _cam;
    [SerializeField]
    private float maxRange = 1000.0f;
    private string range;
    
    RaycastHit hit;

	void Update () {
        
        if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out hit,maxRange))
        {
            range = Vector3.Distance(_cam.transform.position, hit.point).ToString("F1") + "m";
            Vector3 leftPoint = hit.point + _cam.transform.right * - (rangeIconInterval / 2);
            Vector3 rightPoint = hit.point + _cam.transform.right * (rangeIconInterval / 2);
            spacerWidth = _cam.WorldToScreenPoint(rightPoint).x - _cam.WorldToScreenPoint(leftPoint).x; 
        }
        else
        {
            range = "-----m";
        }

        rangeIcon.spacing = spacerWidth;
        rangeDisp.text = range;

	}
}
