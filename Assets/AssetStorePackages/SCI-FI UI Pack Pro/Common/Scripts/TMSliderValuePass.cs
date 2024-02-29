using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TMSliderValuePass: MonoBehaviour {

    //[SerializeField]Text progress;
    [SerializeField]TextMeshProUGUI progress;

	// Use this for initialization
	void Start () {
		progress = GetComponent<TextMeshProUGUI>();

	}
	
	public  void UpdateProgress (float content) {
		progress.text = Mathf.Round( content*100) +"%";
	}


}
