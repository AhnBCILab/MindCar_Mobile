using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PUN_TurnManager_auto : MonoBehaviourPun
{
    public GameObject TurnBox;

    void Update()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            TurnBox.GetComponent<Text>().text = GameManager.playerTurncounts[0].ToString();
            if (GameManager.playerTurncounts[0] == 3) GameManager.playerTurncounts[0] = 0;
        }
        else
        {
            TurnBox.GetComponent<Text>().text = "" + GameManager.playerTurncounts[1].ToString();
            if (GameManager.playerTurncounts[1] == 3) GameManager.playerTurncounts[1] = 0;
        }
    }
}
