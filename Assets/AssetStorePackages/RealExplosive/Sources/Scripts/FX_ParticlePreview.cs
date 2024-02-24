using UnityEngine;
using System.Collections;

public class FX_ParticlePreview : MonoBehaviour
{

	public GameObject[] Particles;
	public float RotationSpeed = 3;
	public int Index;
	public Texture2D logo;
	
	void Start ()
	{
	
	}
	
	public void AddParticle (Vector3 position)
	{
		if (Index >= 0 && Index < Particles.Length && Particles.Length > 0) {
			GameObject.Instantiate (Particles [Index], position, Particles [Index].transform.rotation);
		}
	}

	void Update ()
	{
		
		this.transform.Rotate (Vector3.up * RotationSpeed * Time.deltaTime);
		
		if (Input.GetButtonDown ("Fire1")) {
			var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 1000)) {
				AddParticle (hit.point + Vector3.up);
			}
		
		}
		
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			Index += 1;
			if (Index >= Particles.Length || Index < 0)
				Index = 0;
		}
		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			Index -= 1;
			if (Index < 0)
				Index = Particles.Length - 1;
		}
		if (Index >= Particles.Length || Index < 0)
			Index = 0;
	}
	
	void OnGUI(){
		string FXname = "";
		if (Index >= 0 && Index < Particles.Length && Particles.Length > 0) {
			FXname = Particles [Index].name;
		}
		GUI.Label(new Rect(30,30,Screen.width,100),"Change FX : Key Up / Down \nCurrent FX "+FXname);
		if(logo)
			GUI.DrawTexture(new Rect(Screen.width-logo.width-30,30,logo.width,logo.height),logo);
	}
}
