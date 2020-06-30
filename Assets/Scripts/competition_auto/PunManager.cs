using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PunManager : MonoBehaviourPunCallbacks
{

	private readonly string gameVersion = "2";

	public Text connectionInfoText;
	public Button joinButton;
	

	// Use this for initialization
	void Start()
	{
		PhotonNetwork.GameVersion = gameVersion;
		PhotonNetwork.ConnectUsingSettings();

		joinButton.interactable = false;
		connectionInfoText.text = "Connecting To Master Server...";
	}

	// Update is called once per frame
	void Update()
	{

	}

	public override void OnConnectedToMaster()
	{
		joinButton.interactable = true;
		connectionInfoText.text = "Online : Connected to Master Server";
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		joinButton.interactable = false;
		connectionInfoText.text = "Offline : Connection Disabled {cause.ToString()}";

		PhotonNetwork.ConnectUsingSettings();
	}

	public void Connect()
	{
		joinButton.interactable = false;

		if (PhotonNetwork.IsConnected)
		{
			connectionInfoText.text = "Connecting to Random Room...";
			PhotonNetwork.JoinRandomRoom();
		}
		else
		{
			connectionInfoText.text = "Offline : Connection Disabled - Try reconnecting...";

			PhotonNetwork.ConnectUsingSettings();
		}
	}

	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		connectionInfoText.text = "There is no empty room, Creating new Room.";
		PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 });
	}

	public override void OnJoinedRoom()
	{
		connectionInfoText.text = "connected with Room.";
		PhotonNetwork.LoadLevel("4_Auto_Competition");
	}
}
