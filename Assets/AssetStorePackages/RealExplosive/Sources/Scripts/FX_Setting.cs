using UnityEngine;
using System.Collections;

public class FX_Setting : MonoBehaviour {
	
	public float LightIntensityMult = -1;
	public float LifeTime = 2;
	public Light PointLight;
	
	void Start () {
		Destroy(this.gameObject,LifeTime);
		if(PointLight == null){
			if(GetComponent<Light>())
				PointLight = GetComponent<Light>();
		}
	}
	
	
	void Update () {
		if(PointLight!=null){
			PointLight.intensity += LightIntensityMult * Time.deltaTime;
			
			if(PointLight.intensity<0)
				PointLight.intensity = 0;
		}
	}
}
