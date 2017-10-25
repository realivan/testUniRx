using UnityEngine;
using UniRx;
using System;
using System.Collections;

public class ProcessData: MonoBehaviour {
	[SerializeField] public GameObject lightObj;
	[SerializeField] public GameObject explosion;
	static ISubject<string> subject;

	void Start(){
		subject = new Subject<string>();
		subject.Where (x => x == "light on")
			.Subscribe (lightObj.GetComponent<LightObject>().LightOn);
		subject.Where (x => x == "light off")
			.Subscribe (lightObj.GetComponent<LightObject>().LightOff);
		subject.Where (x => x == "explosion")
			.Subscribe (explosion.GetComponent<ExplosionObject>().Explosion);
	}

	public static void Process(string data){
		if (data == null || data.Length < 6)
			return;
		subject.OnNext (data.Substring(0, data.Length-5));
		Server.content = String.Empty;
	}
}
