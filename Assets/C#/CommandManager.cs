using UnityEngine;
using System.Collections;

public class CommandManager : MonoBehaviour {
	Client client;

	void Start(){
		client = GameObject.FindObjectOfType<Client>();
	}

	public void LightOn(){
		client.Send (Client.client, "light on");
	}

	public void LightOff(){
		client.Send (Client.client, "light off");
	}

	public void Explosion(){
		client.Send (Client.client, "explosion");
	}
}
