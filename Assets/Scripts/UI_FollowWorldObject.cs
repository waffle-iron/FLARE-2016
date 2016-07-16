using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(RectTransform))]
public class UI_FollowWorldObject : MonoBehaviour {

    public Transform target;
    public Text distanceMeter;
    public int scaleDistanceThreshold = 1;
    public int scaleMaxDistance = 10;
    public float scaleMinSize = 0.1f;

    private Plane[] planes;
    private Camera _cam;
    private Image _img;
    private RectTransform _rect;
    private Vector3 targetScreenPoint;

	void Awake() {
        _cam = Camera.main;
        _img = GetComponent<Image>();
        _rect = GetComponent<RectTransform>();

        GeometryUtility.CalculateFrustumPlanes(_cam);

        _img.enabled = false;
	}
	
	void Update () {
        targetScreenPoint = _cam.WorldToScreenPoint(target.position);
        float dist = Vector3.Distance(target.position, _cam.transform.position);

        if (GeometryUtility.TestPlanesAABB(planes, target.GetComponent<Collider>().bounds))
        {    
            if (dist > scaleDistanceThreshold)
            {
                float distScaleMod = Mathf.Lerp(1, scaleMinSize, dist / scaleMaxDistance);
                Vector3 scaler = new Vector3(distScaleMod, distScaleMod, distScaleMod);
                _rect.localScale = scaler;
            }
            _rect.position = new Vector3(targetScreenPoint.x,targetScreenPoint.y,0.0f);
            _img.enabled = true;
            if(distanceMeter != null)
            {
                distanceMeter.text = dist.ToString("F2") + "m";
                distanceMeter.enabled = true;
            }

        } else
        {
            if (distanceMeter != null)
            {
                distanceMeter.enabled = false;
            }
            _img.enabled = false;
        }
	}
}
