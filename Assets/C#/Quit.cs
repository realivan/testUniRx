using UnityEngine;
using System.Collections;

public class Quit : MonoBehaviour {

	public void CloseServer(){
		if (Server.listener != null) {
			Server.Close ();
			Application.Quit ();
		}
	}

	public void CloseClient(){
		Client.CloseClient ();
		Application.Quit ();
	}
}
