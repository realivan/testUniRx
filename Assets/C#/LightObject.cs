using UnityEngine;
using System.Collections;

public class LightObject : MonoBehaviour {

	public void LightOn(string cmd){
		Debug.Log ("light on");
		this.GetComponent<Light> ().intensity = 1f;
	}
	
	public void LightOff(string cmd){
		Debug.Log ("light off");
		this.GetComponent<Light> ().intensity = 0f;
	}
}
