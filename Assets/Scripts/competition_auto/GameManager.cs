using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;



public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
	public static GameManager instance;
	public static GameManager Instance
	{
		get
		{
			if (instance == null) instance = FindObjectOfType<GameManager>();

			return instance;
		}
	}

	public Transform[] spawnPositions;
	public GameObject[] playerPrefabs;

	public static string masterID = "";
	public static string clientID = "";
	public static int[] playerTurncounts;


	// Use this for initialization
	void Start()
	{
		if (PhotonNetwork.IsMasterClient) masterID = PhotonNetwork.LocalPlayer.UserId;
		else clientID = PhotonNetwork.LocalPlayer.UserId;

		Debug.Log("masterID: " + masterID);
		Debug.Log("clientID: " + clientID);

		playerTurncounts = new[] { 0, 0 };
		SpawnPlayer();
	}

	private void SpawnPlayer()
	{
		var localPlayerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
		var playerPrefab = playerPrefabs[localPlayerIndex % spawnPositions.Length];
		var spawnPosition = spawnPositions[localPlayerIndex % spawnPositions.Length];

		PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition.position, spawnPosition.rotation);
	}

	public override void OnLeftRoom()
	{
		SceneManager.LoadScene("0_Start_View");
	}

	public override void OnLeftLobby()
	{
		SceneManager.LoadScene("0_Start_View");
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		string tempS = "";
		int tempI = 0;

		if (stream.IsWriting)
		{
			// We own this player: send the others our data
			stream.SendNext(masterID);
			stream.SendNext(clientID);
			stream.SendNext(playerTurncounts[0]);
			stream.SendNext(playerTurncounts[1]);
			//Debug.Log("send the others our data");
		}
		else if (stream.IsReading)
		{
			// Network player, receive data
			tempS = stream.ReceiveNext().ToString();
			if (!tempS.Equals("")) masterID = tempS;
			tempS = stream.ReceiveNext().ToString();
			if (!tempS.Equals("")) clientID = tempS;
			tempI = (int)stream.ReceiveNext();
			if (tempI != 0) playerTurncounts[0] = tempI;
			tempI = (int)stream.ReceiveNext();
			if (tempI != 0) playerTurncounts[1] = tempI;
			//Debug.Log("receive data");
		}
	}
}
