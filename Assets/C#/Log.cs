using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Collections;

public class Log: MonoBehaviour {
	static int count;

	public static void SetText(string msg){
		Observable.WhenAll()
			.ObserveOnMainThread ()
			.Subscribe (_ => GameObject.Find ("log").GetComponent<Text>().text = count.ToString ()+". "+msg);
		count ++;
	}
}
