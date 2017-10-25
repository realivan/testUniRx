using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

public class Client : MonoBehaviour {
	private const int port = 4543;
	private const string host = "localhost";
	private static ManualResetEvent connectDone = new ManualResetEvent(false);
	private static String response = String.Empty;
	public static Socket client;
	private bool canLoadNextLevel;

	public void StartClient() {
		try {
			DontDestroyOnLoad (this.gameObject);
			IPHostEntry ipHostInfo = Dns.GetHostEntry(host);
			IPAddress ipAddress = ipHostInfo.AddressList[0];
			IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
			client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			client.BeginConnect( remoteEP, new AsyncCallback(ConnectCallback), client);
			connectDone.WaitOne(1000);
		} catch (Exception e) {
			Log.SetText(e.ToString());
		}
		if (canLoadNextLevel) {
			LoadNextScene ("client");
			canLoadNextLevel = false;
		}
	}
	
	private void ConnectCallback(IAsyncResult ar) {
		try {
			Socket client = (Socket) ar.AsyncState;
			client.EndConnect(ar);
			Log.SetText("Socket connected to "+ client.RemoteEndPoint.ToString());
			connectDone.Set();
			canLoadNextLevel = true;
			Receive(client);
		} catch (Exception e) {
			Log.SetText(e.ToString());
		}
	}
	#region
	private static void Receive(Socket client) {
		try {
			StateObject state = new StateObject();
			state.workSocket = client;
			client.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
		} catch (Exception e) {
			Debug.Log(e.ToString());
			CloseClient ();
		}
	}

	private static void ReceiveCallback( IAsyncResult ar ) {
		try {
			StateObject state = (StateObject) ar.AsyncState;
			Socket client = state.workSocket;
			int bytesRead = client.EndReceive(ar);
			if (bytesRead > 0) {
				state.sb.Append(Encoding.UTF8.GetString(state.buffer, 0, bytesRead));
				client.BeginReceive(state.buffer , 0, StateObject.BufferSize , 0, new AsyncCallback(ReceiveCallback), state);
			} else {
				if (state.sb.Length > 1) {
					response = state.sb.ToString();
					ProcessData.Process(response);
				}else {
					CloseClient();
					return;
				}
			}
			if(client != null)
				Receive(client);
		} catch (Exception e) {
			Debug.Log(e.ToString());
			CloseClient ();
		}
	}
	#endregion

	public void Send(Socket client, String data) {
		if (client == null)
			return;
		byte[] byteData = Encoding.UTF8.GetBytes(data+"<EOF>");
		client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);
		Log.SetText(data);
	}	

	private void SendCallback(IAsyncResult ar) {
		try {
			Socket client = (Socket) ar.AsyncState;
			client.EndSend(ar);
		} catch (Exception e) {
			Log.SetText(e.ToString());
		}
	}

	public static void CloseClient(){
		if (client == null)
			return;
		Log.SetText("server close the connection");
		client.Shutdown(SocketShutdown.Both);
		client.Close();
		client = null;
	}

	private void LoadNextScene(string scene){
		Application.LoadLevel (scene);
	}
}