using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartManager : MonoBehaviourPun//, IPunObservable
{

	//public bool IsMasterClientLocal = PhotonNetwork.IsMasterClient && photonView.IsMine;
	public GameObject CountdownManager;
	public GameObject CountdownUI;
	public GameObject GOSB;
	public Button StartButton;
	public static bool StartBool = false;

	public PhotonView PV;
	public GameObject TurnManager;

	float timer = 0.0f;
	int waitingTime = 10;

	//public Text TurncountText;
	//public static string masterID = "";
	//public static string clientID = "";

	// Use this for initialization
	void Start () {

		//Debug.Log("masterID: " + masterID);
		//Debug.Log("clientID: " + clientID);

		if (!photonView.IsMine) return;
			
		TurnManager.SetActive(true);

		if (!(PhotonNetwork.IsMasterClient && photonView.IsMine)) return;

		GOSB.SetActive(true);
		StartButton.interactable = false;
	}
	
	// Update is called once per frame
	void Update () {
		//TurncountText.text = masterID + " : " + clientID + " " + GameManager.playerTurncounts[0].ToString() + " : " + GameManager.playerTurncounts[1].ToString();

		if (PhotonNetwork.PlayerList.Length != 2) return;

		timer += Time.deltaTime;

		if (timer > waitingTime)
		{
			StartButton.interactable = true;
			//timer = 0;
		}
	}

	public void onClickStartButton() {
		PV.RPC("startCountdown", RpcTarget.All, null);
		GOSB.SetActive(false);
	}

	[PunRPC]
	void startCountdown()
    {
		//CountdownUI.SetActive(true);
		CountdownManager.SetActive(true);
	}
	/*
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		string tempS = "";
		//int tempI = 0;

		if (stream.IsWriting)
		{
			// We own this player: send the others our data
			stream.SendNext(masterID);
			stream.SendNext(clientID);
			//stream.SendNext(playerTurncounts[0]);
			//stream.SendNext(playerTurncounts[1]);
			//Debug.Log("send the others our data");
		}
		else if (stream.IsReading)
		{
			// Network player, receive data
			tempS = stream.ReceiveNext().ToString();
			if (!tempS.Equals("")) masterID = tempS;
			tempS = stream.ReceiveNext().ToString();
			if (!tempS.Equals("")) clientID = tempS;
			//tempI = (int)stream.ReceiveNext();
			//if (tempI != 0) playerTurncounts[0] = tempI;
			//tempI = (int)stream.ReceiveNext();
			//if (tempI != 0) playerTurncounts[1] = tempI;
			//Debug.Log("receive data");
		}

		//TurncountText.text = masterID + " : " + clientID + " " + playerTurncounts[0].ToString() + " : " + playerTurncounts[1].ToString();
	}*/
}
