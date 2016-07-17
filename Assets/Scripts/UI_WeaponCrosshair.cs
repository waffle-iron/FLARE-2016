using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_WeaponCrosshair : MonoBehaviour {

    public  float spread = 0.0f;
    public bool force = false;

    [SerializeField]
    private Player_WeaponADSAdjust ADS;
    [SerializeField]
    private RectTransform spreadBox;
    [SerializeField]
    private GameObject hitMarkerPrefab;

    void Update()
    {
        spreadBox.gameObject.SetActive(!ADS.isADS || force);

        spreadBox.sizeDelta = new Vector3(spread, spread, 1.0f);
        if (Input.GetKeyDown(KeyCode.H))
        {
            hitMarker();
        }
    }

    public void hitMarker()
    {
        GameObject h = Instantiate(hitMarkerPrefab);
        h.transform.SetParent(spreadBox, false);
    }

}
