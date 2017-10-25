using UnityEngine;
using UniRx;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Server: MonoBehaviour {		
	public static ManualResetEvent allDone = new ManualResetEvent(false);
	private const int port = 4543;
	private const string host = "localhost";
	public static bool run { get; set;}
	public static string content;
	public static Socket listener;
	public static List<Socket> clients;
	
	void Start(){
		clients = new List<Socket> ();
		Observable.EveryUpdate ().Where(_ => content != String.Empty )
			.Subscribe (_ => ProcessData.Process(content));
	}

	public void StartServer() {
		DontDestroyOnLoad (this.gameObject);
		IPHostEntry ipHostInfo = Dns.GetHostEntry(host);
		IPAddress ipAddress = ipHostInfo.AddressList[0];
		IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);
		listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
		try {
			listener.Bind(localEndPoint);
			listener.Listen(10);
			LoadNextScene("server");
			Debug.Log("Waiting for a connection...");
			listener.BeginAccept( new AsyncCallback(AcceptCallback), listener );
		} catch (Exception e) {
			Log.SetText(e.ToString());
		}		
	}
	
	public static void AcceptCallback(IAsyncResult ar) {
		Socket listener = (Socket) ar.AsyncState;
		Socket handler = listener.EndAccept(ar);
		StateObject state = new StateObject();
		state.workSocket = handler;
		clients.Add (handler);
		Debug.Log("connect");
		StartReceive (handler, state);
		listener.BeginAccept( new AsyncCallback(AcceptCallback), listener);
	}

	private static void StartReceive(Socket handler, StateObject state)
	{
		handler.BeginReceive(state.buffer, 0, 200, SocketFlags.None, new AsyncCallback(ReadCallback), state);
		Debug.Log ("Start receive");
	}

	public static void ReadCallback(IAsyncResult ar) {
		content = String.Empty;
		StateObject state = (StateObject) ar.AsyncState;
		Socket handler = state.workSocket;
		int bytesRead = handler.EndReceive(ar);		
		if (bytesRead > 0) {
			state.sb.Append (Encoding.ASCII.GetString (state.buffer, 0, bytesRead));
			content = state.sb.ToString ();
			if (content.IndexOf ("<EOF>") > -1) {
				Debug.Log ("<-- " + content.Substring (0, content.Length - 5));
				state.sb.Remove(0, state.sb.Length);
				StartReceive (handler, state);
			} else {
				StartReceive (handler, state);
			}
		}
	}

	private static void LoadNextScene(string scene){
		Application.LoadLevel (scene);
	}

	public static void Close(){
		foreach(Socket key in clients){
			key.Close ();
		}
		listener.Close ();
		Debug.Log ("server shutdown");
	}
}
