using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_HitMarkerAnim : MonoBehaviour {

    [SerializeField]
    private RectTransform _rect;
    [SerializeField]
    private Image[] arms;
    [SerializeField]
    private float fadeTime;
    [SerializeField]
    private float scaleAmount;

    private float _lerp = 1.0f;

    void Start()
    {
        foreach (Image i in arms)
        {
            i.CrossFadeAlpha(0.0f, fadeTime / 2, false);
        }
    }

	void FixedUpdate () {

        _lerp -= (fadeTime * Time.deltaTime);

        _lerp = Mathf.Clamp01(_lerp);

        float scaler = 25 + (scaleAmount * (1 - _lerp));
        _rect.sizeDelta = new Vector3(scaler, scaler, 1);   
        
        if(_lerp == 0)
        {
            Destroy(this.gameObject);
        }    
	}
}
