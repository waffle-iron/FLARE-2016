using UnityEngine;
using System.Collections;

public class Player_WeaponADSAdjust : MonoBehaviour {

    [Header("ADS Target Aquisition")]
    [SerializeField]
    private GameObject activeWeaponObject;
    private Transform activeWeaponADSTarget;
    [SerializeField]
    private string targetName;

    [Header("ADS Variables")]
    public Camera FPSCamera;
    public float ADS_fov = 30.0f;
    public float ADS_speed = 1.0f;
    public float ADS_eyeDistance = 0.3f;
    public AnimationCurve ADSCurve;
    public AnimationCurve FOVCurve;

    private float _lerp = 0.0f;
    private float _originalFov = 0.0f;
    private Vector3 originPos = new Vector3(0, 0, 0);
    private Quaternion originRot;
    private Vector3 targetPos = new Vector3(0, 0, 0);
    private Quaternion targetRot;
    public bool isADS
    {
        get
        {
            return (activeWeaponObject != null && activeWeaponADSTarget != null && Input.GetButton("Zoom"));
        }
    }

    void Start()
    {
        if(FPSCamera != null) { _originalFov = FPSCamera.fieldOfView; }
        originRot = Quaternion.Euler(Vector3.zero);
    }

	void Update () {
        if (FPSCamera != null)
        {
            if (activeWeaponObject != null && activeWeaponObject.activeInHierarchy)
            {
                if (activeWeaponADSTarget == null)
                {

                    targetPos = originPos;

                    bool hasADS = false;
                    foreach(Transform t in activeWeaponObject.transform)
                    {
                        if(t.name == targetName) {
                            hasADS = true;
                        }
                    }

                        if (hasADS)
                        {
                            activeWeaponADSTarget = activeWeaponObject.transform.FindChild(targetName);
                            Vector3 distance = activeWeaponObject.transform.localPosition + activeWeaponADSTarget.localPosition;
                            Vector3 rotationDiff = activeWeaponObject.transform.localRotation.eulerAngles + activeWeaponADSTarget.localRotation.eulerAngles;

                            targetPos = -distance;
                            targetPos.z += ADS_eyeDistance;

                            targetRot = Quaternion.Euler(-rotationDiff);

                        }
                }
            }
        }
	}

    void FixedUpdate()
    {
        transform.localPosition = Vector3.Lerp(originPos, targetPos, ADSCurve.Evaluate(_lerp));
        transform.localRotation = Quaternion.Lerp(originRot, targetRot, ADSCurve.Evaluate(_lerp));
        FPSCamera.fieldOfView = Mathf.Lerp(_originalFov, ADS_fov, FOVCurve.Evaluate(_lerp));
        _lerp = Mathf.Clamp01(_lerp);

        if (isADS)
        {
            _lerp += ADS_speed * Time.deltaTime;
        }else{
            _lerp -= ADS_speed * 0.5f * Time.deltaTime;
        }
    }
}
